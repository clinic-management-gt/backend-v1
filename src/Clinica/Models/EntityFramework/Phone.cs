using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class Phone
{
    public int Id { get; set; }

    public int ContactId { get; set; }

    public string Phone1 { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Contact Contact { get; set; } = null!;
}
