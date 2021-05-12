using System.ComponentModel.DataAnnotations;

namespace Account.Models
{
    public class AgentLogin
    {
        [Required]
        public string AgentUserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string AgentPassword { get; set; }

        [Required]
        public string Department { get; set; }

    }
}