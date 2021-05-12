using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Models
{
    public class AgentRegister
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        //public int AgentId { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        public string AgentId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "please fill the field")]
        [Column(TypeName = "varchar")]
        public string AgentFirstName { get; set; }

        [Required(ErrorMessage = "please fill the field")]
        [Column(TypeName = "varchar")]
        public string AgentLastName { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "please enter valid date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public DateTime? AgentDoB { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        public string AgentGender { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "please enter valid number")]
        public long AgentContactNumber { get; set; }

        //[Required(ErrorMessage = "please fill the field")]
        //[Column(TypeName = "varchar")]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        //public string AgentEmail { get; set; }

        [Required(ErrorMessage = "please fill the field")]
        [Column(TypeName = "varchar")]
        [DataType(DataType.Password, ErrorMessage = "please enter valid password")]
        public string AgentPassword { get; set; }

        [Required(ErrorMessage = "please fill the field")]
        [Column(TypeName = "varchar")]
        public string Department { get; set; }
    }
}