using System.ComponentModel.DataAnnotations;

namespace User_Management.Model
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Username is required")]
        public string? Username {  get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public string? ContactNumber { get; set; }

        public string Role { get; set; }
    }

    public class LoginModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
