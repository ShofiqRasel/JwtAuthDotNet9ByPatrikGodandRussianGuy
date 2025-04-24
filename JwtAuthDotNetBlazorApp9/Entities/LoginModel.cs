using System.ComponentModel.DataAnnotations;

namespace JwtAuthDotNetBlazorApp9.Entities
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
