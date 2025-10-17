using System.ComponentModel.DataAnnotations;

namespace Clinica.Models;

/// <summary>
/// DTO para información de contacto de un paciente pendiente
/// </summary>
public class PendingPatientContactDTO
{
    /// <summary>
    /// Nombre del contacto
    /// </summary>
    [Required(ErrorMessage = "Contact name is required")]
    [MaxLength(255, ErrorMessage = "Contact name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parentesco/relación con el paciente (ej: "Madre", "Padre", "Tutor", "Familiar")
    /// </summary>
    [Required(ErrorMessage = "Relationship is required")]
    [MaxLength(50, ErrorMessage = "Relationship cannot exceed 50 characters")]
    public string Relationship { get; set; } = string.Empty;

    /// <summary>
    /// Número de teléfono del contacto
    /// </summary>
    [Required(ErrorMessage = "Contact phone is required")]
    [MaxLength(15, ErrorMessage = "Contact phone cannot exceed 15 characters")]
    public string Phone { get; set; } = string.Empty;
}
