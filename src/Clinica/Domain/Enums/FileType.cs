using NpgsqlTypes;

namespace Clinica.Domain.Enums;

/// <summary>
/// Tipos de archivo que un paciente puede subir al sistema.
/// </summary>
public enum FileType
{
    [PgName("receta")]
    Receta,

    [PgName("hoja_de_informacion")]
    HojaDeInformacion,

    [PgName("examen")]
    Examen,

    [PgName("laboratorio")]
    Laboratorio,

    [PgName("radiografia")]
    Radiografia,

    [PgName("resultado_de_laboratorio")]
    ResultadoDeLaboratorio,

    [PgName("consentimiento")]
    Consentimiento,

    [PgName("otro")]
    Otro
}

