using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebUI.Converters
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format = "dd-MM-yyyy HH:mm";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateString = reader.GetString();
            if (string.IsNullOrEmpty(dateString))
            {
                throw new JsonException("Invalid date string");
            }

            return DateTime.ParseExact(dateString, _format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
}
