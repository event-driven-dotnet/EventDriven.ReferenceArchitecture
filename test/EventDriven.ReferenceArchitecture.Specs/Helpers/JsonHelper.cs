using System.Text.Json;

namespace EventDriven.ReferenceArchitecture.Specs.Helpers;

public static class JsonHelper
{
    public static string JsonPrettify(this string json)
    {
        using var jDoc = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
    }
}