namespace TeleNeuro.Service.UserService.Models
{
    public class UserRegisterModel
    {
        public int Id { get; set; }
        public int? CreateUserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string[] RoleKey { get; set; }
    }
}
