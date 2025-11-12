namespace Clinica.Application.DTOs.Requests;

/// <summary>
/// Payload para registrar un nuevo conjunto de mediciones de crecimiento.
/// </summary>
public class GraphMeasurementRequestDTO
{
    public int PatientId { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal HeadCircumference { get; set; }
}
