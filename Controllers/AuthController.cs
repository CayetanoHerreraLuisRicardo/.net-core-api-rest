using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestSlabon.Models.Request;
using TestSlabon.Models.Response;
using TestSlabon.Services;

namespace TestSlabon.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _service;
        public AuthController(IUserService userService)
        {
            _service = userService;
        }
        /// <summary>
        /// Endpoint de login 
        /// </summary>
        /// <remarks>Aqui es donde él usuario se autentica, necesitas enviar el `email` y `contraseña` de un usuario registrado, la información es validada, si es un usuario válido la API regresa como respuesta los datos del usuario con el `token`.</remarks>
        /// <response code="200">Operación exitosa</response>
        /// <response code="400">Indica que el servidor no puede o no procesará la solicitud debido a algo que se percibe como un error del cliente </response>
        /// <response code="500">Oops! Error en el servidor </response>
        [HttpPost, Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessResponseLogin), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest _model)
        {
            SuccessResponseLogin success;
            try
            {
                LoginResponse oRes = await _service.Auth(_model);
                if (oRes == null)
                {
                    string[] aErrors = { "Usuario o contraseña incorrecta" };
                    return BadRequest(new ErrorResponse(aErrors, 1));
                }
                success = new SuccessResponseLogin
                {
                    Data = oRes,
                    Message = "Bienvenido " + _model.UserName
                };
                return Ok(success);
            }
            catch (Exception e)
            {
                string[] aErrors = { e.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(aErrors, -1));
            }
        }

    }
}
