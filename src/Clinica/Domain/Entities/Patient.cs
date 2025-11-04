namespace Clinica.Domain.Entities;

public partial class Patient
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly Birthdate { get; set; }

    public DateOnly LastVisit { get; set; }

    public string Address { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public int BloodTypeId { get; set; }

    public int PatientTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual BloodType BloodType { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<PatientInsurance> PatientInsurances { get; set; } = new List<PatientInsurance>();

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<PatientAlergy> PatientAlergies { get; set; } = new List<PatientAlergy>();

    public virtual ICollection<PatientChronicDisease> PatientChronicDiseases { get; set; } = new List<PatientChronicDisease>();

    public virtual ICollection<PatientExam> PatientExams { get; set; } = new List<PatientExam>();

    public virtual PatientType PatientType { get; set; } = null!;

    public virtual ICollection<PatientVaccine> PatientVaccines { get; set; } = new List<PatientVaccine>();
}
