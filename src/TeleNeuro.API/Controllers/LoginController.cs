using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlayCore.Core.CustomException;
using PlayCore.Core.Logger;
using PlayCore.Core.Managers.JWTAuthenticationManager;
using PlayCore.Core.Model;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly IBasicLogger<IUserService> _basicLogger;

        public LoginController(IUserService userService, IJwtAuthenticationManager jwtAuthenticationManager, IUserManagerService userManagerService, IBasicLogger<IUserService> basicLogger)
        {
            _userService = userService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _userManagerService = userManagerService;
            _basicLogger = basicLogger;
        }
        [HttpPost]
        public async Task<BaseResponse<JwtTokenResult>> Login(UserLoginModel model)
        {
            _basicLogger.LogInfo("User Login", model);
            var (user, roles) = await _userService.Login(model);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                roles?.ForEach(i => claims.Add(new Claim(ClaimTypes.Role, i.Key)));
                return new BaseResponse<JwtTokenResult>().SetResult(_jwtAuthenticationManager.Generate(user.Id.ToString(), claims));
            }

            throw new UIException("Kullanıcı girişi başarısız");
        }
        [HttpPost]
        [Authorize]
        public BaseResponse<bool> Logout()
        {
            _jwtAuthenticationManager.RemoveRefreshToken(_userManagerService.UserId.ToString());
            return new BaseResponse<bool>().SetResult(true);
        }
        [HttpPost]
        [Authorize]
        public BaseResponse<JwtTokenResult> RefreshToken(RefreshTokenModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.RefreshToken))
                    throw new UIException("Yenileme anahtarı bulunamadı.").SetResultCode(401);
                return new BaseResponse<JwtTokenResult>().SetResult(_jwtAuthenticationManager.Refresh(model.RefreshToken, _userManagerService.Token, _userManagerService.UserId.ToString()));
            }
            catch (SecurityTokenException e)
            {
                throw new UIException(e.Message).SetResultCode(401);
            }
        }
    }
}
