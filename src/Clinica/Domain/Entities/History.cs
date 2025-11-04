namespace Clinica.Domain.Entities;

public partial class History
{
    public int Id { get; set; }

    public string TableName { get; set; } = null!;

    public int RecordId { get; set; }

    public DateTime ChangedAt { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }
}
