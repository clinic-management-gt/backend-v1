namespace Clinica.Models
{
    public class ContactResponseDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public required string Type { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public List<PhoneResponseDTO> Phones { get; set; } = new();
    }

    public class PhoneResponseDTO
    {
        public int ContactId { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
