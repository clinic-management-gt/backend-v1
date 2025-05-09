
namespace Clinica.Models;
public class DashBoardDTO
{
    public string PatientName { get; set; }
    public string DoctorName { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }

    public DashBoardDTO(string patientName, string doctorName, DateTime date, string status)
    {
       PatientName = patientName;
       DoctorName = doctorName;
       Date = date;
       Status = status;
    }
}


