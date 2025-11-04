namespace Clinica.Application.DTOs.Requests;

/// <summary>
/// DTO para crear un paciente pendiente de confirmar
/// </summary>
public class CreatePendingPatientDTO
{
    /// <summary>Nombre del paciente</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Apellido del paciente</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Fecha de nacimiento del paciente</summary>
    public DateTime Birthdate { get; set; }

    /// <summary>Género del paciente</summary>
    public string Gender { get; set; } = string.Empty;

    /// <summary>Lista de contactos del paciente</summary>
    public List<PendingPatientContactDTO>? Contacts { get; set; }
}

/// <summary>
/// DTO para contacto de paciente pendiente
/// </summary>
public class PendingPatientContactDTO
{
    /// <summary>Tipo de contacto (Padre, Madre, Tutor, etc.)</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Nombre completo del contacto</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Número de teléfono del contacto</summary>
    public string PhoneNumber { get; set; } = string.Empty;
}

