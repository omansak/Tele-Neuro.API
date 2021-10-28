using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using System.Threading.Tasks;
using PlayCore.Core.CustomException;
using PlayCore.Core.Managers.JWTAuthenticationManager;
using TeleNeuro.Service.UserService;
using TeleNeuro.Service.UserService.Models;

namespace TeleNeuro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController
    {
        private readonly IUserService _userService;
        private readonly IJWTAuthenticationManager _jwtAuthenticationManager;

        public LoginController(IUserService userService, IJWTAuthenticationManager jwtAuthenticationManager)
        {
            _userService = userService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }
        [HttpPost]
        public async Task<BaseResponse> Login(UserLoginModel model)
        {
            var (user, roles) = await _userService.Login(model);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                roles?.ForEach(i => claims.Add(new Claim(ClaimTypes.Role, i.Key)));
                return new BaseResponse().SetResult(_jwtAuthenticationManager.Generate(user.Id.ToString(), claims));
            }

            throw new UIException("Kullanıcı girişi başarısız");
        }
        [HttpPost]
        public async Task<BaseResponse> Register(UserRegisterModel model)
        {
            return new BaseResponse().SetResult(await _userService.Register(model));
        }
    }
}
