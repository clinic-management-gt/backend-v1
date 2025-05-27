using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Models;

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




}
