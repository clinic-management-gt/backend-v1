using Clinica.Domain.Enums;

namespace Clinica.Application.DTOs.Responses;

/// <summary>
/// Representa un punto de datos para los gráficos de crecimiento.
/// </summary>
public class GraphDataPointDTO
{
    /// <summary>
    /// Edad calculada en días desde el nacimiento (eje X).
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Valor de la medición correspondiente al eje Y.
    /// </summary>
    public decimal Y { get; set; }

    public DateTime MeasuredAt { get; set; }
}

/// <summary>
/// Respuesta que agrupa la información del paciente y los puntos del gráfico solicitado.
/// </summary>
public class GraphResponseDTO
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public ChartRange RequestedRange { get; set; }
    public ChartType RequestedType { get; set; }
    public List<GraphDataPointDTO> Data { get; set; } = new();
}
