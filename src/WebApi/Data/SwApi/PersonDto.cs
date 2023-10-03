namespace WebApi.Data.SwApi;

using System.Text.Json.Serialization;

public sealed class PersonDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("birth_year")]
    public string BirthYear { get; set; }

    [JsonPropertyName("eye_color")]
    public string EyeColor { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; }

    [JsonPropertyName("hair_color")]
    public string HairColor { get; set; }

    [JsonPropertyName("height")]
    public string Height { get; set; }

    [JsonPropertyName("mass")]
    public string Mass { get; set; }

    [JsonPropertyName("skin_color")]
    public string SkinColor { get; set; }

    [JsonPropertyName("homeworld")]
    public string Homeworld { get; set; }

    [JsonPropertyName("films")]
    public List<string> Films { get; set; }

    [JsonPropertyName("species")]
    public List<string> Species { get; set; }

    [JsonPropertyName("spaceships")]
    public List<string> Spaceships { get; set; }

    [JsonPropertyName("vehicles")]
    public List<string> Vehicles { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }

    [JsonPropertyName("edited")]
    public DateTimeOffset Edited { get; set; }
}
