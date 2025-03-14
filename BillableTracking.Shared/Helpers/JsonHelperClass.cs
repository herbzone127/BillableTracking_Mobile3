using System.Text.Json;
using System.Text.Json.Serialization;

namespace BillableTracking.Shared.Helpers
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };


        public static string Serialize<T>(T value)
        {
            try
            {
                return JsonSerializer.Serialize(value, DefaultOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization failed: {ex.Message}");
                Console.WriteLine($"Path: {ex.Path}");
                Console.WriteLine($"LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
            return string.Empty;
        }

        public static T Deserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, DefaultOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization failed: {ex.Message}");
                Console.WriteLine($"Path: {ex.Path}");
                Console.WriteLine($"LineNumber: {ex.LineNumber}, BytePositionInLine: {ex.BytePositionInLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
            return default!;
        }
    }
}
