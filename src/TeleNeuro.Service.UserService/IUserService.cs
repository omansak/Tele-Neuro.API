using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Service.UserService.Models;

namespace TeleNeuro.Service.UserService
{
    public interface IUserService
    {
        ConcurrentBag<Role> RoleDefinition { get; }
        Task<bool> Register(UserRegisterModel model);
        Task<(User User, List<Role> Roles)> Login(UserLoginModel model);
    }
}