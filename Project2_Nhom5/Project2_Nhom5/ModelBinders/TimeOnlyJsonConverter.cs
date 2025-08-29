using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Project2_Nhom5.ModelBinders
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string Format = "HH:mm";

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var timeString = reader.GetString();
                if (TimeOnly.TryParseExact(timeString, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
                {
                    return time;
                }
                if (TimeOnly.TryParse(timeString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime))
                {
                    return parsedTime;
                }
            }
            
            throw new JsonException($"Unable to convert \"{reader.GetString()}\" to TimeOnly.");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }
    }
} 