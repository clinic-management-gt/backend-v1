namespace Clinica.Models
{
    public class ContactResponseDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<PhoneResponseDTO> Phones { get; set; } = new();
    }

    public class PhoneResponseDTO
    {
        public int ContactId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
