using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Clinica.Domain.Entities;
    /// <summary>
    /// Entidad para teléfonos de contactos de pacientes pendientes
    /// </summary>
    [Table("pending_patient_phones")]
    public partial class PendingPatientPhone
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("pending_patient_contact_id")]
        public int PendingPatientContactId { get; set; }

        [Required]
        [Column("phone")]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // Navegación
        public virtual PendingPatientContact? PendingPatientContact { get; set; }
    }
