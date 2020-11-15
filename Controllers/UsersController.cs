using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestSlabon.Models.Entities;
using TestSlabon.Models.Request;
using TestSlabon.Models.Response;
using TestSlabon.Services;

namespace TestSlabon.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _service;
        public UsersController(IUserService userService)
        {
            _service = userService;
        }
        /// <summary>
        /// Listar usuario
        /// </summary>
        /// <remarks>Aqui es donde podras consultar la lista de usuarios.</remarks>
        /// <response code="200">Operación exitosa</response>
        /// <response code="400">Indica que el servidor no puede o no procesará la solicitud debido a algo que se percibe como un error del cliente </response>
        /// <response code="500">Oops! Error en el servidor </response>
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessResponseUsers), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            SuccessResponseUsers success;
            try
            {
                success = new SuccessResponseUsers
                {
                    Data = await _service.GetAll()
                };
                return Ok(success);
            }
            catch (Exception e)
            {
                string[] aErrors = { e.Message };
                return StatusCode(500, new ErrorResponse(aErrors, -1));
            }
        }

        /// <summary>
        /// Consulta usuario por id
        /// </summary>
        /// <param name="id"></param>
        ///         [Consumes(MediaTypeNames.Application.Json)]
        /// <response code="200">Operación exitosa</response>
        /// <response code="400">Indica que el servidor no puede o no procesará la solicitud debido a algo que se percibe como un error del cliente </response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Oops! Error en el servidor </response>
        [HttpGet("{id}", Name = "GetById")]
        [Authorize]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessResponseUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            SuccessResponseUser success;
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse(Utils.Helper.GetErrors(ModelState), 1));
                }
                success = new SuccessResponseUser();
                UserResponse oUser = await _service.GetById(id);
                success.Data = oUser;
                if (oUser == null)
                {
                    string[] aErrors = { Utils.Constants.NOT_FOUND };
                    return NotFound(new ErrorResponse(aErrors, 1));
                }
                return Ok(success);
            }
            catch (Exception e)
            {
                string[] aErrors = { e.Message };
                return StatusCode(500, new ErrorResponse(aErrors, -1));
            }
        }

        /// <summary>
        /// Editar usuario por id
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Operación exitosa</response>
        /// <response code="400">Indica que el servidor no puede o no procesará la solicitud debido a algo que se percibe como un error del cliente </response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Oops! Error en el servidor </response>
        [HttpPut("{id}")]
        [Authorize]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        /* NOTA: Para este servicio se agregó la validación del email o username en el servicio*/
        public async Task<IActionResult> Put(int id, [FromBody] UserRequest model)
        {
            SuccessResponse success = new SuccessResponse();
            try {
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(new ErrorResponse(Utils.Helper.GetErrors(ModelState), 1));
                //}
                if (id != model.PkuserId)
                {
                    string[] aErrors = { $"El valor de id '{id}' no corresponde con el id del objeto '{model.PkuserId}'." };
                    return BadRequest(new ErrorResponse(aErrors, 1));
                }
                int updated = await _service.Update(model);
                switch (updated)
                {
                    case -3:
                        string[] aErrors3 = { Utils.Constants.USERNAME_EXIST };
                        return BadRequest(new ErrorResponse(aErrors3, 1));
                    case -2:
                        string[] aErrors0 = { Utils.Constants.EMAIL_EXISTS };
                        return BadRequest(new ErrorResponse(aErrors0, 1));
                    case -1:
                        string[] aErrors1 = { "Error Al intentar, actualizar los datos" };
                        return StatusCode(500, new ErrorResponse(aErrors1, -1));
                    case 0:
                        string[] aErrors2 = { Utils.Constants.NOT_FOUND };
                        return NotFound(new ErrorResponse(aErrors2, 1));
                }
                success.Message = "Usuario modificado con exito";
                return Ok(success);
            }
            catch(Exception e)
            {
                string[] aErrors = { e.Message };
                return StatusCode(500, new ErrorResponse(aErrors, -1));
            }
        }

        /// <summary>
        /// Listar usuario
        /// </summary>
        /// <response code="201">Recurso creado con exito</response>
        /// <response code="400">Indica que el servidor no puede o no procesará la solicitud debido a algo que se percibe como un error del cliente </response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Oops! Error en el servidor </response>
        /* NOTA: Para este servicio se agregó la validación del email o username en el controller*/
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessResponseUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(UserRequest model)
        {
            try {
                if (await _service.UserNameExists(model))
                {
                    string[] aErrors = { Utils.Constants.USERNAME_EXIST };
                    return BadRequest(new ErrorResponse(aErrors, 1));
                }
                if (await _service.EmailExists(model.Email))
                {
                    string[] aErrors = { Utils.Constants.EMAIL_EXISTS };
                    return BadRequest(new ErrorResponse(aErrors, 1));
                }
                model.Password = Utils.Helper.GetSHA256(model.Password);
                Users userAdded = await _service.Add(model);
                UserResponse newUser = new UserResponse();
                newUser.PkuserId = userAdded.PkuserId;
                newUser.Email = userAdded.Email;
                newUser.UserName = userAdded.UserName;
                newUser.PkuserId = userAdded.PkuserId;
                newUser.PkuserId = userAdded.PkuserId;
                SuccessResponseUser response = new SuccessResponseUser
                {
                    Data = newUser,
                    Message = "Usuario registrado con exito"
                };
                return CreatedAtAction(actionName: "GetById", controllerName: "Users", routeValues: new { id = newUser.PkuserId }, response);
            }
            catch(Exception e)
            {
                string[] aErrors = { e.Message };
                return StatusCode(500, new ErrorResponse(aErrors, -1));
            }
        }

        /// <summary>
        /// Listar usuario
        /// </summary>
        /// <response code="200">Operación exitosa</response>
        /// <response code="400">Indica que el servidor no puede o no procesará la solicitud debido a algo que se percibe como un error del cliente </response>
        /// <response code="404">Recurso no encontrado</response>
        /// <response code="500">Oops! Error en el servidor </response>
        [HttpDelete("{id}")]
        [Authorize]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            SuccessResponse success = new SuccessResponse();
            try {
                UserResponse oUser = await _service.GetById(id);
                if (oUser == null || !oUser.Status)
                {
                    string[] aErrors = { Utils.Constants.NOT_FOUND };
                    return NotFound(new ErrorResponse(aErrors, 1));
                }
                await _service.Delete(oUser);
                success.Message = "Usuario eliminado con exito";
                return Ok(success);
            } 
            catch(Exception e) {
                string[] aErrors = { e.Message };
                return StatusCode(500, new ErrorResponse(aErrors, -1));
            }
        }
    }
}
