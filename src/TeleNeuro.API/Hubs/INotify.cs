using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using TeleNeuro.Entities;
using TeleNeuro.Service.MessagingService.Models;

namespace TeleNeuro.API.Hubs
{
    public interface INotify
    {
        Task NotifyNewMessage(ConversationMessage conversationMessage);
        Task NotifyNewConversation(ConversationSummary conversationSummary);
        Task NotifyReadConversation(int userId, int conversationId);
    }
}
