using System;
using System.Collections.Generic;

namespace SistemaGimnasio.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdMembresia { get; set; }

    public DateOnly FechaPago { get; set; }

    public decimal Monto { get; set; }

    public virtual Membresia IdMembresiaNavigation { get; set; } = null!;
}
