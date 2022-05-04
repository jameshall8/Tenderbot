using System.Text.RegularExpressions;
using MargieBot;

namespace SlackMessageBot;

public class TenderbotResponder : IResponder
{
    public bool CanRespond(ResponseContext context)
    {
        return
            Regex.IsMatch(context.Message.Text, "\bTenderbot\b") &&
            context.Message.ChatHub.Type == SlackChatHubType.Channel &&
            context.UserNameCache[context.Message.User.ToString()] == "obnoxious coworker" &&
            DateTime.Now.Minute > 30;
        
    }

    public BotMessage GetResponse(ResponseContext context)
    {
        return new BotMessage()
        {
            Text = "Hello" + context.Message.User + ", and Hello Tenderbot"
        };
    }
}