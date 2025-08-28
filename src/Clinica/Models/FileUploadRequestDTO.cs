using Microsoft.AspNetCore.Http;

namespace Clinica.Models.EntityFramework
{
    /// <summary>
    /// Modelo para recibir los datos de la solicitud de subida de archivos de pacientes.
    /// </summary>
    public class FileUploadRequest
    {
        /// <summary>
        /// Identificador del paciente al que pertenece el archivo.
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Tipo de documento (ejemplo: receta, examen, etc).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Descripción opcional del archivo.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Archivo a subir (formato multipart/form-data).
        /// </summary>
        public IFormFile File { get; set; } = null!;

        /// <summary>
        /// Identificador del expediente médico relacionado.
        /// </summary>
        public int MedicalRecordId { get; set; }
    }
}
