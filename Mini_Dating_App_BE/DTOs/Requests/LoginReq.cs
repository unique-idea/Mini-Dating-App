using System.ComponentModel.DataAnnotations;

namespace Mini_Dating_App_BE.DTOs.Requests
{
    public class LoginReq
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }
}
