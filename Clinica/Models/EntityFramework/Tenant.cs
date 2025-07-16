using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class Tenant
{
    public int Id { get; set; }

    public string DbName { get; set; } = null!;

    public string DbHost { get; set; } = null!;

    public string DbUser { get; set; } = null!;

    public string DbPassword { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
