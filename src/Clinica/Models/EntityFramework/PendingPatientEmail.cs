using System;

namespace Clinica.Models.EntityFramework;

public partial class PendingPatientEmail
{
    public int Id { get; set; }

    public int PendingPatientContactId { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual PendingPatientContact PendingPatientContact { get; set; } = null!;
}
