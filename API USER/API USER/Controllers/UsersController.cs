using Model.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using System;
using API_USER.Service.IServices;
using API_USER.Service.Services;
using Model.Models.ViewModels;
using Model.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace API_USER.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {


        //INICIALICAMOS EL DBCONTEXT
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }
        [HttpPost]
        [Route("Register")]

        public IActionResult Register([FromBody] UsuarioViewModel usuario)
        {
            try
            {
                 _service.RegisterUser(usuario);
                return StatusCode(StatusCodes.Status201Created, new { mensaje = "Confirma el el correo recibido a su direccion para que se registre correctamente!." });
                
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = mensajeError });

            }

            
        }



        [HttpPost]
        [Route("Login")]
        
        public IActionResult Login([FromBody] LoginViewModel login )
        {
           


            if(login.Correo == "")
            {
                return BadRequest("Ingrese un correo");
            }

            if (login.Clave == "")
            {
                return BadRequest("Ingrese una contraseña");
            }
            try
            {


                string token = _service.LoginUser(login);



                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Has entrado con exito!" , response = token});


            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = mensajeError });

            }
        }

        [HttpGet]
        [Route("User/{email}")]
        [Authorize]
        public IActionResult GetUserByEmail(string email)
        {
           

            try
            {

                UsuarioDTO usuario = _service.GetUserByEmail(email);

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Usuario Encontrado!", response = usuario });


            }
            catch (Exception ex) {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = mensajeError });
            }
        }




        [HttpPut]
        [Route("changepw/{token}")]
        [Authorize]
        public IActionResult ChangePassword(CPassViewModel changepw)
        {

            try
            {
                _service.ChangePassword(changepw);

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Usuario Modificado!" });


            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = mensajeError });
            }
        }




        [HttpPut]
        [Route("ConfirmationMail")]

        public IActionResult ConfirmUser(string token)
        {

            try
            {
                _service.ConfirmUser(token);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Usuario confirmado!" });


            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = mensajeError });
            }
        }


        [HttpGet("confirmation")]
        public IActionResult ConfirmarCuenta(string token)
        {
            _service.confirmMail(token);
            return StatusCode(StatusCodes.Status200OK, new { mensaje = "Usuario confirmado!" });

        }


    }
}
