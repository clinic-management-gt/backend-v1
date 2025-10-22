using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class PendingPatient
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly Birthdate { get; set; }

    public string Gender { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PendingPatientContact> PendingPatientContacts { get; set; } = new List<PendingPatientContact>();
}
