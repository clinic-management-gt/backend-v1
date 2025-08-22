using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class Recipe
{
    public int Id { get; set; }

    public int TreatmentId { get; set; }

    public required string Prescription { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Treatment Treatment { get; set; } = null!;
}
