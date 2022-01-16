using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Service.MessagingService.Models;

namespace TeleNeuro.Service.MessagingService
{
    public interface IConversationService
    {
        Task<ConversationSummary> CreateConversation(CreateConversationModel model);
        Task<List<ConversationSummary>> UserConversations(int userId, int? conversationId = null, bool includeEmpty = false);
        Task<ConversationMessage> InsertMessage(InsertMessageModel model);
        Task<ConversationMessageInfo> ConversationMessages(ConversationMessageModel model, int pageSize = 10);
        Task<bool> ReadConversationAllMessages(int conversationId, int userId);
        Task<List<ConversationParticipant>> ConversationParticipants(int conversationId);
        Task<int> UserUnreadConversationCount(int userId);
    }
}