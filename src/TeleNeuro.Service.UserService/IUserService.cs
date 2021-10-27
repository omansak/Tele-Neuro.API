using System.Threading.Tasks;
using TeleNeuro.Entities;
using TeleNeuro.Service.UserService.Models;

namespace TeleNeuro.Service.UserService
{
    public interface IUserService
    {
        Task<bool> Register(UserRegisterModel model);
        Task<Login> Login(UserLoginModel model);
    }
}