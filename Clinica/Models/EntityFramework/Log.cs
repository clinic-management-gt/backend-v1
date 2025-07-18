using System;
using System.Collections.Generic;

namespace Clinica.Models.EntityFramework;

public partial class Log
{
    public int Id { get; set; }

    public string TableName { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public int? UserId { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public virtual User? User { get; set; }

    public LogAction action { get; set; }
}
