using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Models
{
    public class Register
    {
        [Key]
        [Column(TypeName = "varchar")]
        public string UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "please fill the field")]
        [Column(TypeName = "varchar")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "please fill the field")]
        [Column(TypeName = "varchar")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "please enter valid date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public DateTime? DoB { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        public string Gender { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "please enter valid number")]
        public long ContactNumber { get; set; }

        [Required(ErrorMessage = "please fill the field")]
        [Column(TypeName = "varchar")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "please fill the field")]
        [Column(TypeName = "varchar")]
        [DataType(DataType.Password, ErrorMessage = "please enter valid password")]
        public string Password { get; set; }
    }
}