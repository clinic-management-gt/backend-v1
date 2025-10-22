namespace Clinica.Models;

public class PendingPatientCreateDTO
{
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required DateTime Birthdate { get; set; }
    public required string Gender { get; set; }
    public required List<PendingPatientContactDTO> Contacts { get; set; }
}

public class PendingPatientContactDTO
{
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required List<string> Phones { get; set; }
}
