using System.ComponentModel.DataAnnotations;

namespace User_Management.Model
{
    public class Signup
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
}
