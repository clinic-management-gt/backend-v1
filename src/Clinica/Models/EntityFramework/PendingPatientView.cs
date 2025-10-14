using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinica.Models.EntityFramework;

/// <summary>
/// Vista de base de datos para pacientes pendientes de confirmación
/// Esta clase mapea a una vista SQL que filtra pacientes donde confirmed_at IS NULL
/// </summary>
[Table("pending_patients_view")]
public class PendingPatientView
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
