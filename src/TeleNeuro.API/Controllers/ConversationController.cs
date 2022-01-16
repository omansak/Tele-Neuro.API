using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.CustomException;
using PlayCore.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.API.Hubs;
using TeleNeuro.API.Services;
using TeleNeuro.Service.MessagingService;
using TeleNeuro.Service.MessagingService.Models;

namespace TeleNeuro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ConversationController
    {
        private readonly IUserManagerService _userManagerService;
        private readonly IConversationService _conversationService;
        private readonly INotificationHubService _notificationHubService;

        public ConversationController(IUserManagerService userManagerService, IConversationService conversationService, INotificationHubService notificationHubService)
        {
            _userManagerService = userManagerService;
            _conversationService = conversationService;
            _notificationHubService = notificationHubService;
        }

        [HttpPost]
        public async Task<BaseResponse<ConversationSummary>> CreateConversation(CreateConversationModel model)
        {
            model.UserId = _userManagerService.UserId;
            return new BaseResponse<ConversationSummary>().SetResult(await _conversationService.CreateConversation(model));
        }

        [HttpGet]
        public async Task<BaseResponse<List<ConversationSummary>>> UserConversations()
        {
            return new BaseResponse<List<ConversationSummary>>().SetResult(await _conversationService.UserConversations(_userManagerService.UserId));
        }

        [HttpPost]
        public async Task<BaseResponse<ConversationMessage>> InsertMessage(InsertMessageModel model)
        {
            model.UserId = _userManagerService.UserId;
            var result = await _conversationService.InsertMessage(model);
            if (result is { MessageId: > 0 })
            {
                await _notificationHubService.NotifyNewMessage(result);
                return new BaseResponse<ConversationMessage>().SetResult(result);
            }

            throw new UIException("Mesaj gönderilirken bir hata oluştur");
        }

        [HttpPost]
        public async Task<BaseResponse<ConversationMessageInfo>> ConversationMessages(ConversationMessageModel model)
        {
            model.UserId = _userManagerService.UserId;
            return new BaseResponse<ConversationMessageInfo>().SetResult(await _conversationService.ConversationMessages(model, 10));
        }

        [HttpPost("{conversationId}")]
        public async Task<BaseResponse<bool>> ReadConversationAllMessages(int conversationId)
        {
            var result = await _conversationService.ReadConversationAllMessages(conversationId, _userManagerService.UserId);
            if (result)
            {
                await _notificationHubService.NotifyReadConversation(_userManagerService.UserId, conversationId);
            }
            return new BaseResponse<bool>().SetResult(result);
        }

        [HttpGet]
        public async Task<BaseResponse<int>> UserUnreadConversationCount()
        {
            return new BaseResponse<int>().SetResult(await _conversationService.UserUnreadConversationCount(_userManagerService.UserId));
        }
    }
}
