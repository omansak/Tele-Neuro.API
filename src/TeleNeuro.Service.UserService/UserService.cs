using PlayCore.Core.CustomException;
using PlayCore.Core.PasswordHasher;
using PlayCore.Core.Repository;
using PlayCore.Core.ValidateHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
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
        public ConcurrentBag<Role> RoleDefinition => _roleDefinitions;
        public async Task<List<UserInfo>> ListFilterUsers(BaseFilterModel model)
        {
            return await GetQueryableFilterUsers(model).ToListAsync();
        }
        public async Task<int> CountFilterUsers(BaseFilterModel model)
        {
            return await GetQueryableFilterUsers(model, true, false).CountAsync();
        }
        public async Task<int> UpdateUser(UserRegisterModel model)
        {
            var validateHelper = new ValidateHelper();
            async Task AddRole(int userId)
            {
                var userRoles = await _baseRepository.ListAsync<UserRoleRelation>(i => i.UserId == userId);
                if (userRoles != null)
                {
                    await _baseRepository.DeleteRangeAsync(userRoles);
                }

                if (model.RoleKey != null)
                {
                    foreach (var roleKey in model.RoleKey)
                    {
                        Role role = _roleDefinitions.SingleOrDefault(i => i.Key == roleKey);
                        if (role != null)
                        {
                            var userRole = await _baseRepository.InsertAsync(new UserRoleRelation
                            {
                                CreatedDate = DateTime.Now,
                                RoleId = role.Id,
                                UserId = userId
                            });

                            if (userRole == null)
                            {
                                throw new UIException("Kullanıcı rolü eklenemedi.");
                            }
                        }
                    }
                    return;
                }
                throw new UIException("Kullanıcı rolü bulanamdı.");
            }

            if (model.Id > 0)
            {
                var user = await _baseRepository.SingleOrDefaultAsync<User>(i => i.Id == model.Id);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.Email) && validateHelper.ValidateEmail(model.Email))
                    {
                        user.Email = model.Email;
                        await _baseRepository.UpdateAsync(user);
                        var userProfile = await _baseRepository.SingleOrDefaultAsync<UserProfile>(i => i.UserId == user.Id);
                        if (userProfile != null)
                        {
                            if (model.RoleKey?.Length == 0)
                            {
                                throw new UIException("Kullanıcı rolü bulanamdı.");
                            }
                            userProfile.Name = model.Name;
                            userProfile.Surname = model.Surname;
                            await _baseRepository.UpdateAsync(userProfile);
                            await AddRole(user.Id);
                            return user.Id;
                        }
                        throw new UIException("Kullanıcı profili bulunamadı.");
                    }
                    throw new UIException("Email boş olamaz yada hatalı format olamaz.");
                }
                throw new UIException("Kullanıcı bulunamadı.");
            }

            if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password))
            {
                if (validateHelper.ValidateEmail(model.Email) && validateHelper.ValidatePassword(model.Password))
                {
                    if (await _baseRepository.AnyAsync<User>(i => i.Email == model.Email))
                    {
                        throw new UIException("Email adresi kullanıyor.");
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
                        var userProfile = await _baseRepository.InsertAsync(new UserProfile
                        {
                            UserId = user.Id,
                            Name = model.Name,
                            Surname = model.Surname
                        });

                        if (userProfile != null)
                        {
                            await AddRole(user.Id);
                            return user.Id;
                        }
                        throw new UIException("Kullanıcı profil eklenemedi.");
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
        private IQueryable<UserInfo> GetQueryableFilterUsers(BaseFilterModel model, bool includeFilter = true, bool includePaging = true)
        {
            return _baseRepository
                .GetQueryable<User>()
                .Join(_baseRepository.GetQueryable<UserProfile>(), i => i.Id, j => j.UserId, (i, j) => new
                {
                    User = i,
                    UserProfile = j
                })
                .Select(i => new UserInfo
                {
                    User = i.User,
                    UserProfile = i.UserProfile,
                    Roles = _baseRepository
                        .GetQueryable<UserRoleRelation>()
                        .Where(j => j.UserId == i.User.Id)
                        .Join(_baseRepository.GetQueryable<Role>(), k => k.RoleId, j => j.Id, (ik, j) => new
                        {
                            Role = j
                        })
                        .Select(k => k.Role.Key)
                        .ToList()
                })
                .ToQueryableFromBaseFilter(model, includeFilter, includePaging);
        }
    }
}
