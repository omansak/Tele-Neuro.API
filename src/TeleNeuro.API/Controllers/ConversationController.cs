using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public ConversationController(IUserManagerService userManagerService, IConversationService conversationService)
        {
            _userManagerService = userManagerService;
            _conversationService = conversationService;
        }

        [HttpPost]
        public async Task<BaseResponse<int>> CreateConversation(CreateConversationModel model)
        {
            model.UserId = _userManagerService.UserId;
            return new BaseResponse<int>().SetResult(await _conversationService.CreateConversation(model));
        }

        [HttpGet]
        public async Task<BaseResponse<List<ConversationSummary>>> UserConversations()
        {
            return new BaseResponse<List<ConversationSummary>>().SetResult(await _conversationService.UserConversations(_userManagerService.UserId));
        }

        [HttpPost]
        public async Task<BaseResponse<bool>> InsertMessage(InsertMessageModel model)
        {
            model.UserId = _userManagerService.UserId;
            return new BaseResponse<bool>().SetResult(await _conversationService.InsertMessage(model));
        }

        [HttpPost]
        public async Task<BaseResponse<ConversationMessageInfo>> ConversationMessages(ConversationMessageModel model)
        {
            model.UserId = _userManagerService.UserId;
            return new BaseResponse<ConversationMessageInfo>().SetResult(await _conversationService.ConversationMessages(model, 10));
        }
    }
}
