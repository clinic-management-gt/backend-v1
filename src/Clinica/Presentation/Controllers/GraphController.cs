using Clinica.Application.DTOs.Requests;
using Clinica.Application.DTOs.Responses;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Clinica.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Presentation.Controllers;

[ApiController]
[Route("graph")]
public class GraphController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GraphController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Registra las mediciones de peso, altura, perímetro cefálico e IMC calculado para un paciente.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RecordGrowthMeasurements([FromBody] GraphMeasurementRequestDTO request)
    {
        if (request.Weight <= 0 || request.Height <= 0 || request.HeadCircumference <= 0)
        {
            return BadRequest(new { message = "Peso, altura y perímetro cefálico deben ser mayores a cero." });
        }

        var patient = await _context.Patients.FindAsync(request.PatientId);
        if (patient == null)
        {
            return NotFound(new { message = $"Paciente con id {request.PatientId} no encontrado." });
        }

        var measuredAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        var heightInMeters = request.Height / 100m;

        List<GrowthMeasurement> measurements = new()
        {
            new GrowthMeasurement
            {
                PatientId = request.PatientId,
                MeasurementType = MeasurementType.Weight,
                Value = decimal.Round(request.Weight, 2),
                MeasuredAt = measuredAt,
                CreatedAt = measuredAt
            },
            new GrowthMeasurement
            {
                PatientId = request.PatientId,
                MeasurementType = MeasurementType.Height,
                Value = decimal.Round(request.Height, 2),
                MeasuredAt = measuredAt,
                CreatedAt = measuredAt
            },
            new GrowthMeasurement
            {
                PatientId = request.PatientId,
                MeasurementType = MeasurementType.HeadCircumference,
                Value = decimal.Round(request.HeadCircumference, 2),
                MeasuredAt = measuredAt,
                CreatedAt = measuredAt
            }
        };

        if (heightInMeters > 0)
        {
            decimal bmiValue = request.Weight / (heightInMeters * heightInMeters);
            bmiValue = decimal.Round(bmiValue, 2, MidpointRounding.AwayFromZero);
            measurements.Add(new GrowthMeasurement
            {
                PatientId = request.PatientId,
                MeasurementType = MeasurementType.BodyMassIndex,
                Value = bmiValue,
                MeasuredAt = measuredAt,
                CreatedAt = measuredAt
            });
        }

        patient.LastVisit = DateOnly.FromDateTime(measuredAt);

        await _context.GrowthMeasurements.AddRangeAsync(measurements);
        await _context.SaveChangesAsync();

        var response = new
        {
            message = "Mediciones guardadas correctamente.",
            patientId = patient.Id,
            recordedAt = measuredAt,
            registeredMeasurements = measurements.Count
        };

        return Ok(response);
    }

    /// <summary>
    /// Devuelve los datos necesarios para graficar una curva específica para un paciente.
    /// </summary>
    [HttpGet("{patientId:int}/{chartRange}/{chartType}")]
    public async Task<ActionResult<GraphResponseDTO>> GetGrowthChart(
        int patientId,
        ChartRange chartRange,
        ChartType chartType)
    {
        var patient = await _context.Patients.FindAsync(patientId);
        if (patient == null)
        {
            return NotFound(new { message = $"Paciente con id {patientId} no encontrado." });
        }

        MeasurementType measurementType = MapChartTypeToMeasurementType(chartType);
        (int MinDays, int MaxDays) ageRange = GetAgeRange(chartRange);
        DateTime birthDateTime = patient.Birthdate.ToDateTime(TimeOnly.MinValue);

        var measurements = await _context.GrowthMeasurements
            .Where(m => m.PatientId == patientId && m.MeasurementType == measurementType)
            .OrderBy(m => m.MeasuredAt)
            .ToListAsync();

        var data = measurements
            .Select(measurement =>
            {
                int ageInDays = Math.Max(0, (int)Math.Round((measurement.MeasuredAt.Date - birthDateTime.Date).TotalDays));
                return new
                {
                    measurement,
                    AgeInDays = ageInDays
                };
            })
            .Where(entry => entry.AgeInDays >= ageRange.MinDays && entry.AgeInDays <= ageRange.MaxDays)
            .Select(entry => new GraphDataPointDTO
            {
                X = entry.AgeInDays,
                Y = decimal.Round(entry.measurement.Value, 2, MidpointRounding.AwayFromZero),
                MeasuredAt = DateTime.SpecifyKind(entry.measurement.MeasuredAt, DateTimeKind.Unspecified)
            })
            .ToList();

        var response = new GraphResponseDTO
        {
            PatientId = patient.Id,
            PatientName = $"{patient.Name} {patient.LastName}".Trim(),
            Age = FormatAgeDescription(patient.Birthdate, DateTime.UtcNow),
            Gender = patient.Gender,
            RequestedRange = chartRange,
            RequestedType = chartType,
            Data = data
        };

        return Ok(response);
    }

    private static (int MinDays, int MaxDays) GetAgeRange(ChartRange chartRange)
    {
        return chartRange switch
        {
            ChartRange.BirthTo13Weeks => (0, 13 * 7),
            ChartRange.BirthTo2Years => (0, 2 * 365),
            ChartRange.BirthTo5Years => (0, 5 * 365),
            ChartRange.BirthTo6Months => (0, 6 * 30),
            ChartRange.SixMonthsToTwoYears => (6 * 30, 2 * 365),
            ChartRange.TwoYearsToFiveYears => (2 * 365, 5 * 365),
            _ => (0, int.MaxValue)
        };
    }

    private static MeasurementType MapChartTypeToMeasurementType(ChartType chartType)
    {
        return chartType switch
        {
            ChartType.WeightForAge => MeasurementType.Weight,
            ChartType.HeightForAge => MeasurementType.Height,
            ChartType.HeadCircumferenceForAge => MeasurementType.HeadCircumference,
            ChartType.BodyMassIndexForAge => MeasurementType.BodyMassIndex,
            _ => throw new ArgumentOutOfRangeException(nameof(chartType), chartType, null)
        };
    }

    private static string FormatAgeDescription(DateOnly birthDate, DateTime reference)
    {
        var referenceDate = DateOnly.FromDateTime(reference);
        var birth = birthDate;
        if (referenceDate < birth)
        {
            return "0 años";
        }

        int years = referenceDate.Year - birth.Year;
        int months = referenceDate.Month - birth.Month;
        int days = referenceDate.Day - birth.Day;

        if (days < 0)
        {
            months--;
            var previousMonth = reference.AddMonths(-1);
            days += DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
        }

        if (months < 0)
        {
            years--;
            months += 12;
        }

        if (years < 0)
        {
            years = 0;
        }

        var parts = new List<string> { $"{years} años" };
        if (months > 0)
        {
            parts.Add($"{months} meses");
        }

        return string.Join(" ", parts);
    }
}
