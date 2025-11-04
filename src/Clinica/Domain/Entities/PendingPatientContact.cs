using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Clinica.Domain.Entities;

/// <summary>
/// Entidad para contactos de pacientes pendientes
/// </summary>
[Table("pending_patient_contacts")]
public partial class PendingPatientContact
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("pending_patient_id")]
    public int PendingPatientId { get; set; }

    [Required]
    [Column("type")]
    [StringLength(20)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("last_name")]
    [StringLength(255)]
    public string LastName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navegación
    public virtual PendingPatient? PendingPatient { get; set; }
    public virtual ICollection<PendingPatientPhone> PendingPatientPhones { get; set; } = new List<PendingPatientPhone>();
}

