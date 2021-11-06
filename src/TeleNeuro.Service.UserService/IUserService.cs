using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayCore.Core.Model;
using TeleNeuro.Entities;
using TeleNeuro.Service.UserService.Models;

namespace TeleNeuro.Service.UserService
{
    public interface IUserService
    {
        ConcurrentBag<Role> RoleDefinition { get; }
        Task<List<UserInfo>> ListFilterUsers(BaseFilterModel model);
        Task<int> CountFilterUsers(BaseFilterModel model);
        Task<int> UpdateUser(UserRegisterModel model);
        Task<(User User, List<Role> Roles)> Login(UserLoginModel model);
    }
}