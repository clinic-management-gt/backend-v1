namespace Clinica.Models;

/// <summary>
/// DTO para crear un paciente básico (solo nombre y apellido)
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
}
