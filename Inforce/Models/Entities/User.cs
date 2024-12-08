using System.ComponentModel.DataAnnotations;

namespace Inforce_Task.Models.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Login is required")]
        [MinLength(6, ErrorMessage = "Min 6 characters")]
        [MaxLength(15, ErrorMessage = "Max 15 characters")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Min 8 characters")]
        [MaxLength(20, ErrorMessage = "Max 20 characters")]
        public string Password { get; set; }
        public string Role { get; set; }

        public User(string login, string password, string role = "User")
        {
            UserId = Guid.NewGuid();
            Login = login;
            Password = password;
            Role = role;
        }
    }
}
