using System.ComponentModel.DataAnnotations;

namespace Account.Models
{
    public class LoginViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}