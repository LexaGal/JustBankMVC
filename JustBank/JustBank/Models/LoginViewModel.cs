using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JustBank.Models
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("Name")]
        public string UserName { get; set; }
       
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}