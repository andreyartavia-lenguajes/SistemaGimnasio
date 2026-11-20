using System;
using System.Collections.Generic;

namespace SistemaGimnasio.Models;

public partial class Asistencia
{
    public int IdAsistencia { get; set; }

    public int IdCliente { get; set; }

    public DateOnly FechaAsistencia { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
