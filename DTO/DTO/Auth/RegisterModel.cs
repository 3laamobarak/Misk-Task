using System.ComponentModel.DataAnnotations;

namespace DTO.DTO.Auth
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required,StringLength(256)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}