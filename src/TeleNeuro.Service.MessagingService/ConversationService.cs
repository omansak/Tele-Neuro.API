using Microsoft.EntityFrameworkCore;
using PlayCore.Core.CustomException;
using PlayCore.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.MessagingService.Models;

namespace TeleNeuro.Service.MessagingService
{
    public class ConversationService : IConversationService
    {
        private readonly IBaseRepository<TeleNeuroDatabaseContext> _baseRepository;

        public ConversationService(IBaseRepository<TeleNeuroDatabaseContext> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<ConversationSummary> CreateConversation(CreateConversationModel model)
        {
            var user = await _baseRepository.SingleOrDefaultAsync<User>(i => i.Id == model.UserId);

            if (user == null)
                throw new UIException("Kullanıcı bulunamadı");


            var participants = model.Participants?.ToList().Where(i => i != user.Id).ToArray();

            if (participants == null || participants.Length == 0)
                throw new UIException("En az bir alıcı gereklidir.");

            var participantUsers = await _baseRepository.ListAsync<User>(i => participants.Contains(i.Id));

            if (participantUsers.Count > 1 && string.IsNullOrWhiteSpace(model.GroupName))
                throw new UIException("Grup konuşmalarında grup ismi zorunludur.");

            if (participantUsers.Count == 1)
            {
                var oldConversation = (
                        await _baseRepository
                            .GetQueryable<ConversationParticipant>()
                            .Where(i => _baseRepository
                                .GetQueryable<ConversationParticipant>()
                                .Join(_baseRepository.GetQueryable<Conversation>(), j => j.ConversationId, k => k.Id,
                                    (j, k) => new
                                    {
                                        Conversation = k,
                                        ConversationParticipant = j
                                    })
                                .Where(j => j.Conversation.IsActive && participantUsers.Select(k => k.Id).Concat(new[] { model.UserId }).ToList().Contains(j.ConversationParticipant.UserId))
                                .Select(j => j.Conversation.Id)
                                .Distinct()
                                .Contains(i.ConversationId))
                            .ToListAsync())
                    .GroupBy(i => i.ConversationId)
                    .Select(i => new
                    {
                        ConversationId = i.Key,
                        Participants = i.Select(j => j.UserId).ToList(),
                        ParticipantCount = i.Count()
                    })
                    .SingleOrDefault(i => i.Participants.Count == 2 && string.Join(",", i.Participants.OrderBy(j => j)) == string.Join(",", participantUsers.Select(j => j.Id).Concat(new[] { model.UserId }).OrderBy(j => j)));

                if (oldConversation != null)
                    return (await UserConversations(user.Id, oldConversation.ConversationId, true)).SingleOrDefault();
            }

            var conversation = await _baseRepository.InsertAsync(new Conversation
            {
                UserId = user.Id,
                CreateDate = DateTime.Now,
                Name = model.GroupName,
                IsActive = true,
                IsGroup = participantUsers.Count > 1
            });

            if (conversation != null)
            {
                foreach (var item in participantUsers.Select(j => j.Id).Concat(new[] { model.UserId }))
                {
                    await _baseRepository.InsertAsync(new ConversationParticipant
                    {
                        ConversationId = conversation.Id,
                        UserId = item
                    });
                }

                return (await UserConversations(user.Id, conversation.Id, true)).SingleOrDefault();
            }

            throw new UIException("Konuşma oluşturulamadı");

        }

        public async Task<List<ConversationSummary>> UserConversations(int userId, int? conversationId = null, bool includeEmpty = false)
        {
            var conversationsIds = await _baseRepository
                .GetQueryable<ConversationParticipant>()
                .Where(i => i.UserId == userId)
                .Select(i => i.ConversationId)
                .Distinct()
                .ToListAsync();

            return (await _baseRepository
                .GetQueryable<Conversation>()
                .Where(i => !conversationId.HasValue || i.Id == conversationId)
                .Join(_baseRepository.GetQueryable<ConversationParticipant>(), i => i.Id, j => j.ConversationId, (i, j) => new
                {
                    Conversation = i,
                    ConversationParticipant = j
                })
                .Join(_baseRepository.GetQueryable<User>(), i => i.ConversationParticipant.UserId, j => j.Id, (i, j) => new
                {
                    Conversation = i.Conversation,
                    ConversationParticipant = i.ConversationParticipant,
                    User = j
                })
                .Join(_baseRepository.GetQueryable<UserProfile>(), i => i.User.Id, j => j.UserId, (i, j) => new
                {
                    Conversation = i.Conversation,
                    ConversationParticipant = i.ConversationParticipant,
                    User = i.User,
                    UserProfile = j
                })
                .Where(i => conversationsIds.Contains(i.Conversation.Id) && i.Conversation.IsActive)
                .Select(i => new
                {
                    Conversation = i.Conversation,
                    ConversationParticipant = i.ConversationParticipant,
                    User = i.User,
                    UserProfile = i.UserProfile,
                    ParticipantUserInfo = new ParticipantUserInfo
                    {
                        User = i.User,
                        UserProfile = i.UserProfile
                    },
                    LastMessage = _baseRepository.GetQueryable<Message>().Where(j => j.ConversationId == i.Conversation.Id).OrderByDescending(j => j.CreateDate).FirstOrDefault(),
                    HasUnread = _baseRepository.GetQueryable<MessageRead>().Any(j => j.ConversationId == i.Conversation.Id && j.IsRead == false && j.UserId == userId)
                })
                .Where(i => includeEmpty || i.Conversation.IsGroup || i.LastMessage != null)
                .ToListAsync())
                .GroupBy(i => i.Conversation.Id)
                .Select(i => new ConversationSummary
                {
                    ConversationId = i.Key,
                    IsGroup = i.FirstOrDefault(j => j.Conversation.Id == i.Key)?.Conversation.IsGroup ?? false,
                    Name = i.FirstOrDefault(j => j.Conversation.Id == i.Key)?.Conversation.Name,
                    LastMessage = i.FirstOrDefault(j => j.Conversation.Id == i.Key)?.LastMessage?.MessageString,
                    LastMessageDate = i.FirstOrDefault(j => j.Conversation.Id == i.Key)?.LastMessage?.CreateDate,
                    HasUnread = i.FirstOrDefault(j => j.Conversation.Id == i.Key)?.HasUnread ?? false,
                    Participants = i.Where(j => j.Conversation.Id == i.Key).Select(j => j.ParticipantUserInfo),
                })
                .OrderByDescending(i => i.LastMessageDate)
                .ToList();
        }

        public async Task<ConversationMessage> InsertMessage(InsertMessageModel model)
        {
            var conversation = await _baseRepository.SingleOrDefaultAsync<Conversation>(i => i.Id == model.ConversationId && i.IsActive);
            var dateNow = DateTime.Now;
            if (conversation == null)
                throw new UIException("Konuşma bulunamadı");

            var conversationParticipants = await _baseRepository.ListAsync<ConversationParticipant>(i => i.ConversationId == conversation.Id);

            if (conversationParticipants == null || conversationParticipants.All(i => i.UserId != model.UserId))
                throw new UIException("Kullanıcı konuşmada bulunamadı");

            var message = await _baseRepository.InsertAsync(new Message
            {
                ConversationId = model.ConversationId,
                UserId = model.UserId,
                MessageString = model.Message,
                CreateDate = dateNow
            });

            foreach (var item in conversationParticipants)
            {
                await _baseRepository.InsertAsync(new MessageRead
                {
                    MessageId = message.Id,
                    ConversationId = model.ConversationId,
                    UserId = item.UserId,
                    IsRead = item.UserId == model.UserId,
                    CreateDate = dateNow,
                    UpdateDate = dateNow
                });
            }

            return new ConversationMessage
            {
                ConversationId = conversation.Id,
                CreateDate = message.CreateDate,
                Message = message.MessageString,
                MessageId = message.Id,
                UserId = message.UserId,
                MessageReads = conversationParticipants.Select(i => new MessageRead
                {
                    UserId = i.UserId,
                    ConversationId = i.ConversationId,
                    MessageId = message.Id,
                    CreateDate = dateNow,
                    IsRead = i.UserId == model.UserId,
                    UpdateDate = dateNow
                })
            };
        }

        public async Task<ConversationMessageInfo> ConversationMessages(ConversationMessageModel model, int pageSize = 10)
        {
            var conversation = await _baseRepository
                .GetQueryable<Conversation>()
                .Join(_baseRepository.GetQueryable<ConversationParticipant>(), i => i.Id, j => j.ConversationId,
                    (i, j) => new
                    {
                        Conversation = i,
                        ConversationParticipant = j
                    })
                .Where(i => i.Conversation.Id == model.ConversationId && i.ConversationParticipant.UserId == model.UserId)
                .Select(i => i.Conversation)
                .FirstOrDefaultAsync();

            if (conversation == null)
                throw new UIException("Konuşma bulunamadı");

            Expression<Func<Message, bool>> predicate = i => i.ConversationId == conversation.Id;

            if (model.Cursor.HasValue)
            {
                predicate = i => i.ConversationId == conversation.Id && i.CreateDate < model.Cursor.Value;
            }

            var messages = await _baseRepository.GetQueryable<Message>()
                .Where(predicate)
                .OrderByDescending(i => i.CreateDate)
                .Select(i => new ConversationMessage
                {
                    ConversationId = i.ConversationId,
                    UserId = i.UserId,
                    MessageId = i.Id,
                    Message = i.MessageString,
                    MessageReads = _baseRepository.GetQueryable<MessageRead>().Where(j => j.ConversationId == i.ConversationId && j.MessageId == i.Id).ToList(),
                    CreateDate = i.CreateDate
                })
                .Take(pageSize)
                .ToListAsync();

            DateTime? nextCursor = null;
            if (messages.Count >= pageSize)
            {
                nextCursor = messages.Last().CreateDate;
            }

            return new ConversationMessageInfo
            {
                ConversationMessage = messages.OrderBy(i => i.CreateDate).ToList(),
                Cursor = nextCursor
            };
        }

        public async Task<bool> ReadConversationAllMessages(int conversationId, int userId)
        {
            return (await _baseRepository.ExecuteSqlRawAsync("UPDATE MESSAGE_READ SET IS_READ = 1, UPDATE_DATE = {0} WHERE CONVERSATION_ID = {1} AND USER_ID = {2} AND IS_READ = 0", DateTime.Now, conversationId, userId) > 0);
        }

        public Task<List<ConversationParticipant>> ConversationParticipants(int conversationId)
        {
            return _baseRepository.ListAsync<ConversationParticipant>(i => i.ConversationId == conversationId);
        }

        public Task<int> UserUnreadConversationCount(int userId)
        {
            return _baseRepository
                .GetQueryable<ConversationParticipant>()
                .Where(i => i.UserId == userId)
                .Select(i => i.ConversationId)
                .Distinct()
                .Select(i => new
                {
                    HasUnread = _baseRepository.GetQueryable<MessageRead>().Any(j => j.ConversationId == i && j.IsRead == false && j.UserId == userId)
                })
                .CountAsync(i => i.HasUnread);
        }
    }
}
