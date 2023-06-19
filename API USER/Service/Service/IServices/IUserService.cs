using Model.Models;
using Model.Models.DTO;
using Model.Models.ViewModels;

namespace API_USER.Service.IServices
{
    public interface IUserService
    {
         void RegisterUser(UsuarioViewModel usuario);

        string LoginUser(LoginViewModel login);

        UsuarioDTO GetUserByEmail(string email);

         void ChangePassword(CPassViewModel changepw);

         void ConfirmUser(string token);

        void confirmMail(string token);


    }
}
