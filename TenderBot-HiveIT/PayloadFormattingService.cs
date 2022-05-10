using Newtonsoft.Json.Linq;

namespace TenderBot_HiveIT;

public class PayloadFormattingService
{
    public string GetSelectedId(string payload)
    {
        JObject obj = JObject.Parse(payload);
        var a = obj["message"];
        var b = a["blocks"][2]["accessory"]["value"].ToString().Trim();
        
        return b;
    }
    
    public List<string> GetSelected(string payload)
    {
        List<string> selectedText = new List<string>();
        JObject obj = JObject.Parse(payload);
        var actions = obj["actions"];

        var actionsKeyValue = actions[0];

        var textArray = actionsKeyValue["selected_options"].ToArray();

        foreach (var token in textArray)
        {
            obj = JObject.Parse(token.ToString());
            var textString = obj["text"];
            var finalText = textString["text"].ToString().Trim();
            selectedText.Add(finalText);
        }

        return selectedText;
    }
    public string GetText(string payload)
    {
        try
        {
            JObject obj = JObject.Parse(payload);
            var actions = obj["actions"];

            var actionsKeyValue = actions[0];

            var textArray = actionsKeyValue["text"].ToString();


            obj = JObject.Parse(textArray);

            var text = obj["text"].ToString();

            return text;
        }
        catch
        {
            return "select";
        }
        
    }
    
    public string GetId(string payload)
    {

        JObject obj = JObject.Parse(payload);
        var actions = obj["actions"];
        
        var actionsKeyValue = actions[0];
        
        var id = actionsKeyValue["value"].ToString();
        
        return id;
    }

    public string GetUsername(string payload)
    {
        JObject obj = JObject.Parse(payload);
        var name = obj["user"]["username"].ToString().Trim();


        return name;
    }
}