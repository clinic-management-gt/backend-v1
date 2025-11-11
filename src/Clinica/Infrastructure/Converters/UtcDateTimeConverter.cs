using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Clinica.Infrastructure.Converters
{
    public class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();
            if (string.IsNullOrEmpty(dateString))
            {
                return DateTime.MinValue;
            }

            if (DateTime.TryParse(dateString, out var date))
            {
                // Si la fecha no tiene información de timezone, asumirla como UTC
                if (date.Kind == DateTimeKind.Unspecified)
                {
                    return DateTime.SpecifyKind(date, DateTimeKind.Utc);
                }
                return date.ToUniversalTime();
            }

            throw new JsonException($"Unable to parse '{dateString}' as DateTime.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Asegurar que siempre se serialice en UTC con formato ISO 8601 y 'Z'
            var utcValue = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
            writer.WriteStringValue(utcValue.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }
    }
}
