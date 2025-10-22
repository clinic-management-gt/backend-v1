namespace Clinica.Models;

public class PendingPatientResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Gender { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<PendingPatientContactResponseDTO> Contacts { get; set; } = new();
}

public class PendingPatientContactResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public List<string> Phones { get; set; } = new();
}
