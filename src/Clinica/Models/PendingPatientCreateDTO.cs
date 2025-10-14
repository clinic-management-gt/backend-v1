using System.ComponentModel.DataAnnotations;

namespace Clinica.Models;

/// <summary>
/// DTO para crear un paciente pendiente de confirmación con datos mínimos
/// </summary>
public class PendingPatientCreateDTO
{
    /// <summary>
    /// Nombre del paciente
    /// </summary>
    [Required(ErrorMessage = "Patient name is required")]
    [MaxLength(50, ErrorMessage = "Patient name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del paciente
    /// </summary>
    [Required(ErrorMessage = "Patient last name is required")]
    [MaxLength(50, ErrorMessage = "Patient last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Información de contacto del paciente (nombre del contacto, parentesco y teléfono)
    /// </summary>
    [Required(ErrorMessage = "Contact information is required")]
    public PendingPatientContactDTO Contact { get; set; } = new();
}
