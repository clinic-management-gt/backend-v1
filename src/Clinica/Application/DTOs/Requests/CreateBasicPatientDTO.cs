namespace Clinica.Application.DTOs.Requests;

/// <summary>
/// DTO para crear un paciente básico (solo nombre, apellido y género)
/// </summary>
public class CreateBasicPatientDTO
{
    /// <summary>
    /// Nombre del paciente
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del paciente
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Género del paciente (masculino, femenino, no_especificado)
    /// </summary>
    public string Gender { get; set; } = "no_especificado";
}
