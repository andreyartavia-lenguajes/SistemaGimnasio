using System;
using System.Collections.Generic;

namespace SistemaGimnasio.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Usuario1 { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public int IdRol { get; set; }

    public virtual Role IdRolNavigation { get; set; } = null!;
}
