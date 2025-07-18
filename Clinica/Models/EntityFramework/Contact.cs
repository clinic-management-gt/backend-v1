﻿using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class Contact
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public string Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
