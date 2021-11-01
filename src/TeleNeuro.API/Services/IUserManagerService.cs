using System.Collections.Generic;

namespace TeleNeuro.API.Services
{
    public interface IUserManagerService
    {
        bool IsAuthenticated { get; }
        int UserId { get; }
        string Token { get; }
        IEnumerable<string> Roles { get; }
    }
}