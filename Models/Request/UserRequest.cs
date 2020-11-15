using System.ComponentModel.DataAnnotations;

namespace TestSlabon.Models.Request
{
    public class UserRequest
    {
        [Required(ErrorMessage = "El campo 'pkUserId' es requerido")]
        public int PkuserId { get; set; }

        [Required(ErrorMessage = "El campo 'email' es requerido")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "El correo electrónico es inválido")]
        [StringLength(50)]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo 'userName' es requerido")]
        [StringLength(50, MinimumLength = 7, ErrorMessage = "La longitud mínima es de 7 y la longitud máxima de 50 para el campo usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El campo 'password' es requerido")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[°|¬!#$%&/()=?\¿¡*+~{^},;.:-_])[A-Za-z\d°|¬!#$%&/()=?\¿¡*+~{^},;.:-_]{8,}$", ErrorMessage = "La contraseña debe contener: al menos una mayúscula, una minúscula, un símbolo, un número y 10 caracteres")]
        [StringLength(50, MinimumLength = 7, ErrorMessage = "La longitud mínima es de 7 y la longitud máxima es de 50 para el campo contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El campo 'gender' es requerido")]
        [RegularExpression(@"M|F",ErrorMessage ="Los valores aceptados en genero son: 'M' o 'F'")]
        public string Gender { get; set; }
    }
}
