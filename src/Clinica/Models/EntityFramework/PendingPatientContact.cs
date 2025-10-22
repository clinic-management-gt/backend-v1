using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class PendingPatientContact
{
    public int Id { get; set; }

    public int PendingPatientId { get; set; }

    public string Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual PendingPatient PendingPatient { get; set; } = null!;

    public virtual ICollection<PendingPatientPhone> PendingPatientPhones { get; set; } = new List<PendingPatientPhone>();

    public virtual ICollection<PendingPatientEmail> PendingPatientEmails { get; set; } = new List<PendingPatientEmail>();
}
