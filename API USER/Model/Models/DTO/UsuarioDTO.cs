namespace Model.Models.DTO
{
    public class UsuarioDTO
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
