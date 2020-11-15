using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TestSlabon.Models.Entities;
using TestSlabon.Models.Response;

namespace TestSlabon.Utils
{
    public class Helper
    {
        /// <summary>
        /// SHA-2 ( Secure Hash Algorithm 2 ) SHA-256
        /// Diseñadas por la Agencia de Seguridad Nacional de los Estados Unidos (NSA)
        /// Los proveedores de Unix y Linux están pasando a usar SHA-2 de 256 y 512 bits para el hash seguro de contraseñas
        /// Varias criptomonedas como Bitcoin usan SHA-256
        /// Encriptación de un solo camino
        /// </summary>
        /// <param name="pass" example="Contr@sen@">Contraseña a encriptar</param>
        public static string GetSHA256(string pass)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(pass));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
        /// <summary>
        /// Obtener la lista de errores del request enviado => ModelState
        /// </summary>
        /// <param name="model">ModelStateDictionary</param>
        public static IEnumerable<string> GetErrors(ModelStateDictionary model)
        {
            IEnumerable<string> errors = model
                                .Where(modelError => modelError.Value.Errors.Count > 0)
                                .Select(x => x.Value.Errors.FirstOrDefault().ErrorMessage);
            return errors;
        }
        /// <summary>
        /// El response a enviar al cliente cn codigo 400
        /// </summary>
        /// <param name="actionContext">ActionContext</param>
        public static  BadRequestObjectResult ErrorResponse(ActionContext actionContext)
        {
            IEnumerable<string> errors = actionContext.ModelState
            .Where(modelError => modelError.Value.Errors.Count > 0)
            .Select(x => x.Value.Errors.FirstOrDefault().ErrorMessage);
            return new BadRequestObjectResult(new ErrorResponse(errors, 1));
        }
        /// <summary>
        /// Genera el token 
        /// </summary>
        /// <param name="user">Modelo Usuario </param>
        /// <param name="configuration">proveedor de configuración para extrar los datos del appsettings.json</param>
        public static string GenarateToken(Users user, IConfiguration configuration)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier as string, user.PkuserId.ToString()),
                new Claim(ClaimTypes.Email as string, user.Email)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["JWT:ExpireDays"]));

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
