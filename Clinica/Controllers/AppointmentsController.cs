using Clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Clinica.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IConfiguration _config;

    public AppointmentsController(IConfiguration config)
    {
        _config = config;
    }

    // GET /appointments?status=pendiente
    [HttpGet]
    public ActionResult<List<DashBoardDTO>> GetAllAppointments([FromQuery] string? status)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            var sql = "SELECT a.id, p.name, p.last_name, u.first_name, u.last_name, a.status, a.appointment_date FROM appointments AS a LEFT JOIN patients AS p ON p.id = a.patient_id LEFT JOIN users AS u ON u.id = a.doctor_id";
            if (!string.IsNullOrEmpty(status))
            {
                sql += " WHERE a.status = CAST(@status AS appointment_status_enum)";
                cmd.Parameters.AddWithValue("status", status);
            }
            cmd.CommandText = sql;

            List<DashBoardDTO> dashBoardDTOs = new List<DashBoardDTO>();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                dashBoardDTOs.Add(new DashBoardDTO
                {
                    Id = reader.GetInt32(0),
                    PatientName = $"{reader.GetString(1)} {reader.GetString(2)}",
                    DoctorName = $"{reader.GetString(3)} {reader.GetString(4)}",
                    Status = reader.GetString(5),
                    Date = reader.GetDateTime(6),
                });
            }
            if (dashBoardDTOs.Count > 0)
            {
                return Ok(dashBoardDTOs);
            }
            else
            {
                return NotFound("No hay citas registradas.");
            }

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
    }

    // GET /appointments/today?status=pendiente
    [HttpGet("today")]
    public ActionResult<List<DashBoardDTO>> GetTodaysAppointments([FromQuery] string? status)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            var sql = "SELECT a.id, p.name, p.last_name, u.first_name, u.last_name, a.status, a.appointment_date FROM appointments AS a LEFT JOIN patients AS p ON p.id = a.patient_id LEFT JOIN users AS u ON u.id = a.doctor_id WHERE a.appointment_date::date = current_date";
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND a.status = CAST(@status AS appointment_status_enum)";
                cmd.Parameters.AddWithValue("status", status);
            }
            sql += " ORDER BY a.appointment_date ASC";
            cmd.CommandText = sql;

            List<DashBoardDTO> dashBoardDTOs = new List<DashBoardDTO>();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                dashBoardDTOs.Add(new DashBoardDTO
                {
                    Id = reader.GetInt32(0),
                    PatientName = $"{reader.GetString(1)} {reader.GetString(2)}",
                    DoctorName = $"{reader.GetString(3)} {reader.GetString(4)}",
                    Status = reader.GetString(5),
                    Date = reader.GetDateTime(6),
                });
            }
            if (dashBoardDTOs.Count > 0)
            {
                return Ok(dashBoardDTOs);
            }
            else
            {
                return NotFound("No hay citas para el día de hoy.");
            }

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
    }

    // PATCH /appointments/{id}
    [HttpPatch("{id}")]
    public ActionResult<List<DashBoardDTO>> UpdateAppointmentStatus(int id, [FromBody] UpdateStatusDTO dto)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var sql = "UPDATE appointments SET status = CAST(@status AS appointment_status_enum) WHERE id = @id RETURNING id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("status", dto.Status);
            cmd.Parameters.AddWithValue("id", id);

            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                return Ok(new { message = "Estado actualizado correctamente." });
            }
            else
            {
                return NotFound("No se encontró la cita.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar el estado: {ex.Message}");
        }
    }

    // PUT /appointments/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateAppointment(int id, [FromBody] AppointmentUpdateDTO appointmentDTO)
    {
        Console.WriteLine($"➡️ Endpoint PUT /appointments/{id} reached (to update appointment)");
        Console.WriteLine($"Data received: PatientId={appointmentDTO.PatientId}, DoctorId={appointmentDTO.DoctorId}, Status={appointmentDTO.Status}");

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Verificar si la cita existe y obtener datos actuales
            var checkSql = "SELECT patient_id, doctor_id, appointment_date FROM appointments WHERE id = @id";
            using var checkCmd = new NpgsqlCommand(checkSql, conn);
            checkCmd.Parameters.AddWithValue("id", id);
            
            int currentPatientId = 0;
            int currentDoctorId = 0;
            DateTime currentDate = DateTime.Now;
            bool appointmentExists = false;
            
            using (var reader = checkCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    appointmentExists = true;
                    currentPatientId = reader.GetInt32(0);
                    currentDoctorId = reader.GetInt32(1);
                    currentDate = reader.GetDateTime(2);
                }
            }

            if (!appointmentExists)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }

            // Construir la consulta SQL de actualización dinámicamente
            var updateParts = new List<string>();
            var parameters = new Dictionary<string, object>();
            
            // Solo actualizar campos que no sean nulos
            if (appointmentDTO.PatientId.HasValue && appointmentDTO.PatientId.Value > 0)
            {
                updateParts.Add("patient_id = @patientId");
                parameters.Add("patientId", appointmentDTO.PatientId.Value);
            }
            
            if (appointmentDTO.DoctorId.HasValue && appointmentDTO.DoctorId.Value > 0)
            {
                updateParts.Add("doctor_id = @doctorId");
                parameters.Add("doctorId", appointmentDTO.DoctorId.Value);
            }
            
            if (appointmentDTO.Date.HasValue)
            {
                updateParts.Add("appointment_date = @appointmentDate");
                parameters.Add("appointmentDate", appointmentDTO.Date.Value);
            }
            
            if (!string.IsNullOrEmpty(appointmentDTO.Status))
            {
                updateParts.Add("status = CAST(@status AS appointment_status_enum)");
                parameters.Add("status", appointmentDTO.Status);
            }
            
            updateParts.Add("reason = @reason");
            parameters.Add("reason", appointmentDTO.Notes ?? (object)DBNull.Value);
            
            updateParts.Add("updated_at = NOW()");
            
            var sql = $"UPDATE appointments SET {string.Join(", ", updateParts)} WHERE id = @id";
            
            using var cmd = new NpgsqlCommand(sql, conn);
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
            cmd.Parameters.AddWithValue("id", id);
            
            var rowsAffected = cmd.ExecuteNonQuery();
            
            if (rowsAffected > 0)
            {
                return Ok(new { message = $"Cita con ID {id} actualizada correctamente." });
            }
            else
            {
                return StatusCode(500, "Error al actualizar la cita.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error updating appointment: {ex.Message}");
            return StatusCode(500, $"Error al actualizar la cita: {ex.Message}");
        }
    }

    // DELETE /appointments/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteAppointment(int id)
    {
        Console.WriteLine($"➡️ Endpoint DELETE /appointments/{id} reached (to delete appointment)");

        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            // Verificar si la cita existe
            var checkSql = "SELECT COUNT(*) FROM appointments WHERE id = @id";
            using var checkCmd = new NpgsqlCommand(checkSql, conn);
            checkCmd.Parameters.AddWithValue("id", id);
            var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

            if (!exists)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }

            // Eliminar la cita
            var sql = "DELETE FROM appointments WHERE id = @id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            var rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok($"Appointment with ID {id} deleted successfully.");
            }
            else
            {
                return StatusCode(500, "Error deleting appointment.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting appointment: {ex.Message}");

            // Si es un error de clave foránea, dar un mensaje más específico
            if (ex.Message.Contains("foreign key constraint"))
            {
                return Conflict($"Cannot delete appointment with ID {id} because it has related records (treatments, medical records, etc.).");
            }

            return StatusCode(500, $"Error deleting appointment: {ex.Message}");
        }
    }

}
