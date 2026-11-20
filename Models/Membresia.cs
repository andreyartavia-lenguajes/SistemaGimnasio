using System;
using System.Collections.Generic;

namespace SistemaGimnasio.Models;

public partial class Membresia
{
    public int IdMembresia { get; set; }

    public int IdCliente { get; set; }

    public int IdPlan { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual PlanesMembresium IdPlanNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
