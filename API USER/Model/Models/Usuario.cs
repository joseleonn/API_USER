using System;
using System.Collections.Generic;

namespace Model.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Clave { get; set; }

    public bool? Restrablecer { get; set; }

    public bool? Confirmado { get; set; }
    public string? Rol { get; set; }

    public string? Token { get; set; }
}
