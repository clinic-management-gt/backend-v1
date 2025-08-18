namespace Clinica.Models
{
    public class ContactResponseDTO
    {
        public required int Id { get; set; }
        public required int PatientId { get; set; }
        public required string Type { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required List<PhoneResponseDTO> Phones { get; set; }
    }

    public class PhoneResponseDTO
    {
        public required int ContactId { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
