using System;
using System.Text.Json.Serialization;

namespace Clinica.Models
{
    public class AppointmentUpdateDTO
    {
        // Acepta cadenas vacías o valores nulos y los convierte a null
        private int? _patientId;
        [JsonIgnore]
        public int? PatientId 
        { 
            get => _patientId; 
            set => _patientId = value.HasValue && value.Value == 0 ? null : value; 
        }
        
        // Esta propiedad adicional permite capturar el valor como string
        [JsonPropertyName("patientId")]
        public string PatientIdString 
        { 
            get => _patientId?.ToString() ?? ""; 
            set 
            {
                if (string.IsNullOrEmpty(value))
                    _patientId = null;
                else if (int.TryParse(value, out int result))
                    _patientId = result;
                else
                    _patientId = null;
            }
        }

        // Hacemos lo mismo con doctorId
        private int? _doctorId;
        [JsonIgnore]
        public int? DoctorId 
        { 
            get => _doctorId; 
            set => _doctorId = value.HasValue && value.Value == 0 ? null : value; 
        }
        
        [JsonPropertyName("doctorId")]
        public string DoctorIdString 
        { 
            get => _doctorId?.ToString() ?? ""; 
            set 
            {
                if (string.IsNullOrEmpty(value))
                    _doctorId = null;
                else if (int.TryParse(value, out int result))
                    _doctorId = result;
                else
                    _doctorId = null;
            }
        }

        public DateTime? Date { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }
}