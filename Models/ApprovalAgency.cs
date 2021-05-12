using System.ComponentModel.DataAnnotations;

namespace Account.Models
{
    public class ApprovalAgency
    {
        [Required]
        public string AdminId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}