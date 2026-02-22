
using Mini_Dating_App_BE.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mini_Dating_App_BE.DTOs.Requests
{


    public class RegisterReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
        public int Age { get; set; }

        [Required]
        public GenderEnum? Gender { get; set; }

        [Required]
        public string Bio { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }

}
