using System.ComponentModel.DataAnnotations;

namespace computerapi.DTO
{
    public class LoginDto
    {
        [Required]
        public string? Username { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; } = string.Empty;
    }
}
