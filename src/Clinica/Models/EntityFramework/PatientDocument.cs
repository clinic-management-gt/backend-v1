using System;
using Clinica.Models.EntityFramework.Enums;

namespace Clinica.Models.EntityFramework
{
    /// <summary>
    /// Entidad que representa un documento asociado a un paciente.
    /// </summary>
    public class PatientDocument
    {
        /// <summary>Identificador único del documento.</summary>
        public int Id { get; set; }

        /// <summary>Identificador del paciente.</summary>
        public int PatientId { get; set; }

        /// <summary>Tipo de documento (ejemplo: receta, examen).</summary>
        public FileType Type { get; set; }

        /// <summary>Descripción opcional del documento.</summary>
        public string? Description { get; set; }

        /// <summary>URL del archivo almacenado.</summary>
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>Id del usuario que subió el archivo.</summary>
        public int? UploadedBy { get; set; }

        /// <summary>Fecha de subida del archivo.</summary>
        public DateTime UploadedAt { get; set; }

        /// <summary>Id del expediente médico relacionado.</summary>
        public int? MedicalRecordId { get; set; }

        // Relaciones
        public Patient Patient { get; set; } = null!;
        public User? UploadedByUser { get; set; }

        /// <summary>Tamaño del archivo en bytes.</summary>
        public long? Size { get; set; }

        /// <summary>Tipo MIME del archivo.</summary>
        public string? ContentType { get; set; }
    }
}
