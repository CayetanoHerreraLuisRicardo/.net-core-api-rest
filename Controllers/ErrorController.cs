using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestSlabon.Models.Response;

namespace TestSlabon.Controllers
{
    [Route("api/error")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Recurso no encontrado
        /// </summary>
        /// <remarks>Es utilizado para cuando no se encuentra un recurso solicitado</remarks>
        /* NOTA: Este endpoint se hizo para mandar un mismo modelo de respuesta a como se maneja para los demas errores*/
        [HttpGet, Route("notfound")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            string[] aError = { "Recurso no encontrado" };
            ErrorResponse oError = new ErrorResponse(aError, 1);
            try
            {
                return NotFound(oError);
            }
            catch (Exception e)
            {
                string[] aErrors = { e.Message };
                return StatusCode(500, new ErrorResponse(aErrors, -1));
            }
        }
    }
}
