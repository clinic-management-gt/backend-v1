namespace Clinica.Application.DTOs.Responses;
public class PatientVaccineDTO
{
    public int Id { get; set; }                      // de patient_vaccines
    public string VaccineName { get; set; } = "";    // de vaccines
    public string Brand { get; set; } = "";
    public string Dosis { get; set; } = "";
    public int? AgeAtApplication { get; set; }
    public DateTime? ApplicationDate { get; set; }
    public DateTime CreatedAt { get; set; }          // de patient_vaccines
}

