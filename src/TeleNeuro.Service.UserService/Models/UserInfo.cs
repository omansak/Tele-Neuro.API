using System.Collections.Generic;
using TeleNeuro.Entities;

namespace TeleNeuro.Service.UserService.Models
{
    public class UserInfo
    {
        public User User { get; set; }
        public UserProfile UserProfile { get; set; }
        public List<Role> Roles { get; set; }
    }
}
