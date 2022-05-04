using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json.Serialization;

namespace HiveIT.Tenderbot;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class Text
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("text")]
    public string text { get; set; }

    [JsonPropertyName("emoji")]
    public bool Emoji { get; set; }
}

public class Option
{
    [JsonPropertyName("text")]
    public Text Text { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public class Accessory
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("options")]
    public List<Option> Options { get; set; }

    [JsonPropertyName("action_id")]
    public string action_id { get; set; }
}

public class Block
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("text")]
    public Text Text { get; set; }

    [JsonPropertyName("accessory")]
    public Accessory Accessory { get; set; }
}

public class Root
{
    [JsonPropertyName("blocks")]
    public List<Block> Blocks { get; set; }
}



