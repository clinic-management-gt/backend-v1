using System;

namespace Clinica.Models
{
    public class Paciente{
      public int Id { get; set; }
      public string Name { get; set; } = string.Empty;
      public string LastName { get; set; } = string.Empty;
      public DateTime Birthdate { get; set; }
      public string Address { get; set; } = string.Empty;
      public string Gender { get; set; } = string.Empty; // Podríamos hacer un Enum en C# también si quieres
      public int BloodTypeId { get; set; }
      public int PatientTypeId { get; set; }
      public DateTime CreatedAt { get; set; }
      public DateTime? UpdatedAt { get; set; }
    }
}
