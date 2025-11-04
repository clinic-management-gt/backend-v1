namespace Clinica.Application.DTOs.Responses;

public class ContactResponseDTO
{
    public required int Id { get; set; }
    public required int PatientId { get; set; }
    public required string Type { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required List<PhoneResponseDTO> Phones { get; set; }
    public required List<EmailDTO> Emails { get; set; }
}

public class PhoneResponseDTO
{
    public required int ContactId { get; set; }
    public required string PhoneNumber { get; set; }
}

public class EmailDTO
{
    public required int EmailId { get; set; }
    public required string Email { get; set; }
}


