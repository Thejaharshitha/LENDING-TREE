using System.ComponentModel.DataAnnotations;

namespace Account.Models
{
    public class LoanAgencyAdmin
    {
        [Required]
        public string LoanAgencyAdminId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string LoanAgencyAdminPassword { get; set; }
    }
}