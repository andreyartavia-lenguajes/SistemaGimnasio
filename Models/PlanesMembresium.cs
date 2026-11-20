using System;
using System.Collections.Generic;

namespace SistemaGimnasio.Models;

public partial class PlanesMembresium
{
    public int IdPlan { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public int DuracionDias { get; set; }

    public virtual ICollection<Membresia> Membresia { get; set; } = new List<Membresia>();
}
