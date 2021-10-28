using PlayCore.Core.CustomException;
using PlayCore.Core.PasswordHasher;
using PlayCore.Core.Repository;
using PlayCore.Core.ValidateHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Entity.Context;
using TeleNeuro.Service.UserService.Models;

namespace TeleNeuro.Service.UserService
{
    public class UserService : IUserService
    {
        private static ConcurrentBag<Role> _roleDefinitions;
        private readonly IBaseRepository<TeleNeuroDatabaseContext> _baseRepository;

        public UserService(IBaseRepository<TeleNeuroDatabaseContext> baseRepository)
        {
            _baseRepository = baseRepository;
            _roleDefinitions ??= new ConcurrentBag<Role>(_baseRepository.GetQueryable<Role>().ToList());
        }

        public async Task<bool> Register(UserRegisterModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password))
            {
                var validateHelper = new ValidateHelper();
                if (validateHelper.ValidateEmail(model.Email) && validateHelper.ValidatePassword(model.Password))
                {
                    if (await _baseRepository.AnyAsync<User>(i => i.Email == model.Email))
                    {
                        throw new UIException("Kullanıcı mevcut.");
                    }

                    var user = await _baseRepository.InsertAsync(new User
                    {
                        Email = model.Email,
                        Password = new PasswordHasher().Hash(model.Password),
                        LastLogin = DateTime.Now,
                        CreatedDate = DateTime.Now,
                    });
                    if (user != null)
                    {
                        Role role = _roleDefinitions.SingleOrDefault(i => i.Key == model.RoleKey);
                        if (role != null)
                        {
                            var userRole = await _baseRepository.InsertAsync(new UserRoleRelation
                            {
                                CreatedDate = DateTime.Now,
                                RoleId = role.Id,
                                UserId = user.Id
                            });

                            if (userRole != null)
                            {
                                return true;
                            }
                            throw new UIException("Kullanıcı rolü eklenemedi.");
                        }
                        throw new UIException("Kullanıcı rolü bulunamadı.");
                    }
                    throw new UIException("Kullanıcı eklenirken hata oluştu.");
                }
                throw new UIException("Email & Şifre formatı hatalı");
            }
            throw new UIException("Email & Şifre boş olamaz.");
        }

        public async Task<(User User, List<Role> Roles)> Login(UserLoginModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password))
            {
                var validateHelper = new ValidateHelper();
                if (validateHelper.ValidateEmail(model.Email) && validateHelper.ValidatePassword(model.Password))
                {
                    var userLogin = await _baseRepository.SingleOrDefaultAsync<User>(i => i.Email == model.Email);
                    if (userLogin != null && new PasswordHasher().Verify(model.Password, userLogin.Password))
                    {
                        var userRoles = await _baseRepository.ListAsync<UserRoleRelation>(i => i.UserId == userLogin.Id);
                        return (userLogin, _roleDefinitions.Where(i => userRoles.Any(j => j.RoleId == i.Id)).ToList());
                    }
                    throw new UIException("Email & Şifre yanlış.");
                }
                throw new UIException("Email & Şifre formatı hatalı");
            }
            throw new UIException("Email & Şifre boş olamaz.");
        }
    }
}
