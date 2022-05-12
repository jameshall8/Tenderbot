using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MargieBot;
namespace SlackMessageBot;

public class Slackbot
{
    
    private static void Main(string[] args)
    {
        
        
        
        var container = new WindsorContainer();
        container.Register(Component.For<IResponder>().ImplementedBy<HelloResponder>());

        var bot = new Bot();
        var responders = container.ResolveAll<IResponder>();
        foreach (var responder in responders)
        {
            bot.Responders.Add(responder);
        }
        var connect = bot.Connect("xoxb-3443791463557-3432100996951-ZIfGAXEQBVICEwyT79e5ELME");

        while (Console.ReadLine() != "close") { }
    }
    
     
    }

