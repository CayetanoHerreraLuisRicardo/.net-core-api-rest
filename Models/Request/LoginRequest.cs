using System.ComponentModel.DataAnnotations;

namespace TestSlabon.Models.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El campo userName es requerido")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El campo password es requerido")]
        public string Password { get; set; }
    }
}
