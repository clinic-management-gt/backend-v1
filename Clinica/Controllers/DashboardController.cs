using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Clinica.Models;

namespace Clinica.Controllers;

[ApiController]
[Route("[controller]")]
public class DashboardController : ControllerBase
{
    private readonly string[] ValidStatus = {"confirmado","pendiente","completado","cancelado", "espera" };

    private readonly IConfiguration _config;

    public DashboardController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public IActionResult GetTodaysAppointments([FromQuery] string? status)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");

        try{
          using var conn = new NpgsqlConnection(connectionString);
          conn.Open();

          NpgsqlCommand cmd = new NpgsqlCommand();
          cmd.Connection = conn;

          var sql = "SELECT p.name, p.last_name, u.first_name, u.last_name, a.status , a.appointment_date FROM appointments AS a LEFT JOIN patients AS p ON p.id = a.patient_id LEFT JOIN users AS u ON u.id = a.doctor_id WHERE a.appointment_date::date  = current_date";
          if(!string.IsNullOrEmpty(status)){
            if(ValidStatus.Contains(status.ToLower())){
                sql += " AND a.status = CAST(@status AS appointment_status_enum)"; 
                cmd.Parameters.AddWithValue("status", status);
            }else{
                return BadRequest($"Query parameter {status} is not valid");
            }
            
          }
          cmd.CommandText = sql;


          List<DashBoardDTO> dashBoardDTOs = new List<DashBoardDTO>();
 
          using var reader = cmd.ExecuteReader();

          while (reader.Read())  
          {
             dashBoardDTOs.Add( new DashBoardDTO  
                     {
                        PatientName = $"{reader.GetString(0)} {reader.GetString(1)}",
                        DoctorName = $"{reader.GetString(2)} {reader.GetString(3)}",                        
                        Status = reader.GetString(4),
                        Date = reader.GetDateTime(5),
                    });

          }
            if(dashBoardDTOs.Count > 0){
                return Ok(dashBoardDTOs); 
            } else{
                return NotFound("No hay entradas para el dashboard de hoy");
            }

        }catch(Exception ex){
          return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
        }
    }
}
