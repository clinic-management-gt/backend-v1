
namespace Clinica.Domain.Entities;

public partial class Permission
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int ModuleId { get; set; }

    public bool? CanView { get; set; }

    public bool? CanEdit { get; set; }

    public bool? CanDelete { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Module Module { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
