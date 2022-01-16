using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using PlayCore.Core.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Service.MessagingService;
using TeleNeuro.Service.MessagingService.Models;

namespace TeleNeuro.API.Hubs
{
    public class NotificationHubService : INotificationHubService
    {
        private readonly IHubContext<NotificationHub, INotify> _hubContext;
        private readonly IConversationService _conversationService;
        private readonly IMemoryCache _memoryCache;

        public NotificationHubService(IHubContext<NotificationHub, INotify> hubContext, IConversationService conversationService, IMemoryCache memoryCache)
        {
            _hubContext = hubContext;
            _conversationService = conversationService;
            _memoryCache = memoryCache;
        }

        public async Task NotifyNewMessage(ConversationMessage conversationMessage)
        {
            try
            {
                foreach (var messageRead in conversationMessage.MessageReads.Where(i => i.UserId != conversationMessage.UserId))
                {
                    if (NotificationHub.Connections.TryGetValue(messageRead.UserId, out var list))
                    {
                        foreach (var item in list)
                        {
                            await _hubContext.Clients.Client(item).NotifyNewMessage(conversationMessage);
                        }
                    }
                }
            }
            catch
            {
                //TODO
            }
        }

        public async Task NotifyReadConversation(int userId, int conversationId)
        {
            try
            {
                if (!_memoryCache.TryGetValue($"CONVERSATION_PARTICIPANT_{conversationId}", out List<ConversationParticipant> participants))
                {
                    participants = await _conversationService.ConversationParticipants(conversationId);
                    if (participants != null)
                    {
                        _memoryCache.AddAbsoluteExp($"CONVERSATION_PARTICIPANT_{conversationId}", participants, 15);
                    }
                    else
                    {
                        return;
                    }
                }

                foreach (var messageRead in participants.Where(i => i.UserId != userId))
                {
                    if (NotificationHub.Connections.TryGetValue(messageRead.UserId, out var list))
                    {
                        foreach (var item in list)
                        {
                            await _hubContext.Clients.Client(item).NotifyReadConversation(userId, conversationId);
                        }
                    }
                }
            }
            catch
            {
                //TODO
            }
        }

        public Task NotifyNewConversation(ConversationSummary conversationSummary)
        {
            throw new NotImplementedException();
        }

    }
}
