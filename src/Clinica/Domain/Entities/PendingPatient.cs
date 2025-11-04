
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinica.Domain.Entities;

    [Table("pending_patients")]
    public partial class PendingPatient
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("last_name")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Column("birthdate")]
        public DateOnly Birthdate { get; set; }

        [Required]
        [Column("gender")]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Column("contact_number")]
        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [Column("contact_type")]
        [StringLength(50)]
        public string? ContactType { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relación de navegación con contactos
        public virtual ICollection<PendingPatientContact> PendingPatientContacts { get; set; } = new List<PendingPatientContact>();
    }

