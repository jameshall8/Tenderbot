using System.Collections.Generic;
using Newtonsoft.Json;

namespace HiveIT.Tenderbot;

public class SlackResponse
{
    //{
    //    "response_type": "in_channel",
    //    "text": "It's 80 degrees right now."
    //}
    [JsonProperty("response_type")] public string ResponseType { get; set; }

    
    public string Text { get; set; }
    
    [JsonProperty("blocks")] public List<Block> blocks { get; set; }

    
    
    
    


}