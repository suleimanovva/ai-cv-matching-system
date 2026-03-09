using System.ComponentModel.DataAnnotations;

namespace CvMatchingSystem.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
      
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; 
    }
}