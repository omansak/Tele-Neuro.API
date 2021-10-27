using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using System.Threading.Tasks;
using TeleNeuro.Service.UserService;
using TeleNeuro.Service.UserService.Models;

namespace TeleNeuro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<BaseResponse> Login(UserLoginModel model)
        {
            return new BaseResponse().SetResult(await _userService.Login(model));
        }
        [HttpPost]
        public async Task<BaseResponse> Register(UserRegisterModel model)
        {
            return new BaseResponse().SetResult(await _userService.Register(model));
        }
    }
}
