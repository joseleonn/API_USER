namespace Model.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string ConfirmarClave { get; set; }
        public bool Restrablecer { get; set; }
        public bool Confirmado { get; set; }
        public string Token { get; set; }

    }
}
