using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using PlayCore.Core.CustomException;
using PlayCore.Core.Managers.JWTAuthenticationManager;
using TeleNeuro.API.Attributes;
using TeleNeuro.API.Models;
using TeleNeuro.API.Services;
using TeleNeuro.Service.UserService;
using TeleNeuro.Service.UserService.Models;

namespace TeleNeuro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController
    {
        private readonly IUserService _userService;
        private readonly IUserManagerService _userManagerService;
        private readonly IJWTAuthenticationManager _jwtAuthenticationManager;

        public LoginController(IUserService userService, IJWTAuthenticationManager jwtAuthenticationManager, IUserManagerService userManagerService)
        {
            _userService = userService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _userManagerService = userManagerService;
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
        [Authorize]
        public BaseResponse Logout()
        {
            _jwtAuthenticationManager.RemoveRefreshToken(_userManagerService.UserId.ToString());
            return new BaseResponse().SetResult(true);
        }
        [HttpPost]
        [Authorize]
        public BaseResponse RefreshToken(RefreshTokenModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.RefreshToken))
                    throw new UIException("Yenileme anahtarı bulunamadı.").SetResultCode(401);
                return new BaseResponse().SetResult(_jwtAuthenticationManager.Refresh(model.RefreshToken, _userManagerService.Token, _userManagerService.UserId.ToString()));
            }
            catch (SecurityTokenException e)
            {
                throw new UIException(e.Message).SetResultCode(401);
            }
        }
        [HttpPost]
        public async Task<BaseResponse> Register(UserRegisterModel model)
        {
            return new BaseResponse().SetResult(await _userService.Register(model));
        }
    }
}
