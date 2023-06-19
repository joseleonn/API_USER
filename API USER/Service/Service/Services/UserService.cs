using API_USER.Service.IServices;
using Microsoft.Extensions.Configuration;
using Model.Models;
using Model.Models.DTO;
using Model.Models.ViewModels;
using Service.Service;
using Service.Service.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API_USER.Service.Services
{
    public class UserService : IUserService
    {
        //INICIALICAMOS EL DBCONTEXT
        public readonly DbapiuserContext _dbcontext;

        private readonly string secretKey;

        public UserService(DbapiuserContext context, IConfiguration config)
        {
            _dbcontext = context;
            secretKey = config.GetSection("settings").GetSection("secretkey").ToString();
        }
        public void RegisterUser(UsuarioViewModel usuario)
        {
            CorreoDTO correoDTO = null;
            Usuario correoExist = _dbcontext.Usuarios.FirstOrDefault(u => u.Correo == usuario.Correo);

            //VALIDACIONES PARA MAIL Y CONTRASENA
            if(correoExist != null)
            {
                throw new Exception("El mail ya esta en uso.Utilice otro.");
            };
            if (usuario.Clave.Length < 6)
            {
                throw new Exception("La contraseña debe tener al menos 6 digitos.");

            };
            if (usuario.Clave != usuario.ConfirmarClave)
            {
                throw new Exception("La contraseña no coincide");

            };

            var usuarioEntity = new Usuario
                {
                    // Asigna las propiedades de la entidad basadas en los datos del DTO
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Clave = UtilityService.ConvertHA256(usuario.Clave),
                    Restrablecer = usuario.Restrablecer,
                    Confirmado = false,
                    Token = UtilityService.GenerateToken(),

                };

            ///MANDAMOS MAIL DE CONFIRMACION AL REGISTRARSE.
            string urlConfirmation = "https://localhost:7176/api/Users/confirmation?token=" + usuarioEntity.Token;
            string contenido = $@"<!DOCTYPE html>
                        <html>
                            <head>
                                <title>Confirmación de cuenta</title>
                            </head>
                            <body>
                                <h1>Confirmación de cuenta</h1>
                                <p>Gracias por registrarte en nuestro sitio. Por favor, haz clic en el siguiente botón para confirmar tu cuenta:</p>
                                <a href=""{urlConfirmation}"" target=""_blank"" style=""display:inline-block;background-color:#007bff;color:#fff;padding:10px 20px;text-decoration:none;border-radius:4px;"">Confirmar cuenta</a>
                            </body>
                        </html>";

            correoDTO = new CorreoDTO()
            {
                Para = usuario.Correo,
                Asunto = "Este es un mail de confirmacion",
                Contenido = contenido,


            };
            CorreoService.Enviar(correoDTO);

            //AGREGAMOS USUARIO A LA BASE DE DATOS
            _dbcontext.Usuarios.Add(usuarioEntity);
                _dbcontext.SaveChanges(); 
        }

        public string LoginUser(LoginViewModel login)
        {

            Usuario usuarioAuth = _dbcontext.Usuarios.FirstOrDefault(u => u.Correo == login.Correo && u.Clave == UtilityService.ConvertHA256(login.Clave));

            if (usuarioAuth != null)
            {
                if(usuarioAuth.Confirmado == true) {


                    var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                    var claims = new ClaimsIdentity();

                    //recibimos el correo para darle permisos atravez del mismo
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, login.Correo));

                    //creamos el token
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddDays(60),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)

                    };

                    //lectura del token 
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                    string tokenCreate = tokenHandler.WriteToken(tokenConfig);

                    return tokenCreate;
                }
                else
                {
                    throw new Exception("Falta confirmar el mail del usuario.");

                }

            }
            else
            {
                throw new Exception("Credenciales de inicio de sesión no válidas");
            }

          

        }


        public UsuarioDTO GetUserByEmail(string email)
        {
            UsuarioDTO usuario = null;

            Usuario usuarioEmail = _dbcontext.Usuarios.FirstOrDefault(u => u.Correo == email);


            usuario = new UsuarioDTO()
            {
                Nombre = usuarioEmail.Nombre,
                Correo = usuarioEmail.Correo,
                Clave = usuarioEmail.Clave,
                Restrablecer = (bool)usuarioEmail.Restrablecer,
                Confirmado = (bool)usuarioEmail.Confirmado,
                Token = usuarioEmail.Token,

            };

            return usuario;
        }

        public void ChangePassword(CPassViewModel changepw)
        {

            Usuario userModify = _dbcontext.Usuarios.FirstOrDefault(u => u.Token == changepw.Token);

            if(userModify !=null)
            {
                userModify.Clave = UtilityService.ConvertHA256(changepw.Clave);
                userModify.Restrablecer = (bool)changepw.Restrablecer;
                userModify.Token = changepw.Token;


                _dbcontext.Usuarios.Update(userModify);
                _dbcontext.SaveChanges();
            }
            else
            {
                throw new Exception("Usuario no encontrado");
            }

        }

        public void ConfirmUser(string token)
        {
            CorreoDTO correoDTO = null;
            

            Usuario userConfirm = _dbcontext.Usuarios.FirstOrDefault(u => u.Token == token);
          
            if (userConfirm!= null)
            {
                string urlConfirmation = "https://localhost:7176/api/Users/confirmation?token=" + token;
                string contenido = $@"<!DOCTYPE html>
                        <html>
                            <head>
                                <title>Confirmación de cuenta</title>
                            </head>
                            <body>
                                <h1>Confirmación de cuenta</h1>
                                <p>Gracias por registrarte en nuestro sitio. Por favor, haz clic en el siguiente botón para confirmar tu cuenta:</p>
                                <a href=""{urlConfirmation}"" target=""_blank"" style=""display:inline-block;background-color:#007bff;color:#fff;padding:10px 20px;text-decoration:none;border-radius:4px;"">Confirmar cuenta</a>
                            </body>
                        </html>";


                correoDTO = new CorreoDTO()
                {
                    Para = userConfirm.Correo,
                    Asunto = "Este es un mail de confirmacion",
                    Contenido = contenido,


                };
                CorreoService.Enviar(correoDTO);

            } else
            {
                throw new Exception("Usuario no encontrado");
            }

        }

        public void confirmMail(string token)
        {
            // Buscar el usuario en la base de datos utilizando el token
            var usuario = _dbcontext.Usuarios.FirstOrDefault(u => u.Token == token);

            if (usuario != null)
            {
                // Actualizar el estado de confirmación y el token del usuario
                usuario.Confirmado = true;
                _dbcontext.SaveChanges();

            }
            else
            {
                throw new Exception("No se encontró ningún usuario con el token proporcionado.");


            }

        }



    }
}
