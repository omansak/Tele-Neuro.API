using PlayCore.Core.CustomException;
using PlayCore.Core.PasswordHasher;
using PlayCore.Core.Repository;
using PlayCore.Core.ValidateHelper;
using System;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.UserService.Models;

namespace TeleNeuro.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<TeleNeuroDatabaseContext> _baseRepository;

        public UserService(IBaseRepository<TeleNeuroDatabaseContext> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<bool> Register(UserRegisterModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password))
            {
                var validateHelper = new ValidateHelper();
                if (validateHelper.ValidateEmail(model.Email) && validateHelper.ValidatePassword(model.Password))
                {
                    if (await _baseRepository.AnyAsync<Login>(i => i.Email == model.Email))
                    {
                        throw new UIException("Kullanıcı mevcut.");
                    }

                    var user = await _baseRepository.InsertAsync(new Login
                    {
                        Email = model.Email,
                        Password = new PasswordHasher().Hash(model.Password),
                        LastLogin = DateTime.Now,
                        CreatedDate = DateTime.Now,
                    });
                    return user != null;
                }

                throw new UIException("Email & Şifre formatı hatalı");
            }
            throw new UIException("Email & Şifre boş olamaz.");
        }

        public async Task<Login> Login(UserLoginModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password))
            {
                var validateHelper = new ValidateHelper();
                if (validateHelper.ValidateEmail(model.Email) && validateHelper.ValidatePassword(model.Password))
                {
                    var userLogin = await _baseRepository.SingleOrDefaultAsync<Login>(i => i.Email == model.Email);
                    if (userLogin != null && new PasswordHasher().Verify(model.Password, userLogin.Password))
                    {
                        return userLogin;
                    }

                    throw new UIException("Email & Şifre yanlış.");
                }

                throw new UIException("Email & Şifre formatı hatalı");
            }
            throw new UIException("Email & Şifre boş olamaz.");
        }
    }
}
