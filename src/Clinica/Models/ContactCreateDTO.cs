namespace Clinica.Models;

public class ContactCreateDTO
{
    public required string Type { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required List<string> Phones { get; set; }
    public required List<string> Emails { get; set; }
}


