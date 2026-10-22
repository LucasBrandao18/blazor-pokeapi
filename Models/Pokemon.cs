using System.Text.Json.Serialization;

namespace BlazorPokeApi.Models;

public class Pokemon
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Nome { get; set; }

    [JsonPropertyName("height")]
    public int? Altura { get; set; }

    [JsonPropertyName("weight")]
    public int? Peso { get; set; }

    [JsonPropertyName("sprites")]
    public PokemonSprites? Sprites { get; set; }
}

public class PokemonSprites
{
    [JsonPropertyName("other")]
    public OutrasImagens? Outras { get; set; }
}

public class OutrasImagens
{
    [JsonPropertyName("official-artwork")]
    public ArteOficial? ArteOficial { get; set; }
}

public class ArteOficial
{
    [JsonPropertyName("front_default")]
    public string? Frente { get; set; }
}
