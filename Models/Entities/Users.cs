using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestSlabon.Models.Entities
{
    public partial class Users
    {
        [Required(ErrorMessage = "El campo pkUserId es requerido")]
        public int PkuserId { get; set; }
        [Required(ErrorMessage = "El campo email es requerido")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo userName es requerido")]
        [StringLength(7)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El campo password es requerido")]
        [HiddenInput(DisplayValue = false)]
        public string Password { get; set; }
        public bool Status { get; set; }
        [Required(ErrorMessage = "El campo gender es requerido")]
        [StringLength(1)]
        public string Gender { get; set; }
        [HiddenInput(DisplayValue = false)]
        public DateTime CreatedAt { get; set; }

        public static implicit operator Users(List<Users> v)
        {
            throw new NotImplementedException();
        }
    }
}
