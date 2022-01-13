using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Service.MessagingService.Models;

namespace TeleNeuro.Service.MessagingService
{
    public interface IConversationService
    {
        Task<int> CreateConversation(CreateConversationModel model);
        Task<List<ConversationSummary>> UserConversations(int userId);
        Task<bool> InsertMessage(InsertMessageModel model);
        Task<ConversationMessageInfo> ConversationMessages(ConversationMessageModel model, int pageSize = 10);
        Task<bool> ReadMessage(int conversationId, int userId);
    }
}