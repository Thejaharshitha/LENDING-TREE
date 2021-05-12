using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Models
{
    public class Loan
    {
        public int LoanId { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        public string UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        public string Occupation { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        public string LoanType { get; set; }

        [Required]
        public double AnnualIncome { get; set; }

        [Required]
        public double LoanAmount { get; set; }

        public string ApprovalAgencyStatus { get; set; }
        public string PickUpStatus { get; set; }
        public string VerificationStatus { get; set; }
        public string LegalStatus { get; set; }
        public string FinalStatus { get; set; }
    }
}