namespace Clinica.Models;

/// <summary>
/// DTO de respuesta para pacientes pendientes de confirmación
/// </summary>
public class PendingPatientResponseDTO
{
    /// <summary>
    /// ID del paciente
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del paciente
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del paciente
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del paciente (Name + LastName)
    /// </summary>
    public string FullName => $"{Name} {LastName}";

    /// <summary>
    /// Información de contacto del paciente
    /// </summary>
    public ContactInfo Contact { get; set; } = new();

    /// <summary>
    /// Fecha de creación del registro temporal
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Clase anidada para información de contacto
    /// </summary>
    public class ContactInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
