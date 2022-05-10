using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Slack.Webhooks;
using TenderBot_HiveIT;

namespace HiveIT.Tenderbot;

public static class SlackInteractivity
{
    [FunctionName("SlackInteractivity")]
    public static Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        var payload = req.Form["payload"];

        var url = "";

        var tableService = new TableStorageDatabaseService();

        var messagingService = new SlackMessagingService();
        
        string text = messagingService.GetText(payload).Trim();
        
        
        

        if (text == "More Info")
        {
            string id = messagingService.GetId(payload).Trim();
            messagingService.SendMoreInfoToSlack(new MoreDetails(), id);   
        }
        else if (text == "Send To Favourites")
        {
            string id = messagingService.GetId(payload).Trim();
            if (tableService.CheckIfNew(id) == false)
            {
                url = tableService.GetUrlForMoreInfo(id);
                
                var scrapingService = new ScrapingDetailsRetrievalService(tableService);

                Details details = scrapingService.GetNewPageOverviewDetails(url);
                messagingService.SendToTenderbotSlack(details);
                
            }
        }
        else if (text == "select")
        {
            var selected = messagingService.GetSelected(payload);
            
            
        }
        SlackResponse response = new SlackResponse()
        {
            ResponseType = "in_channel",
            Text = "Done",
        };
        return Task.FromResult<IActionResult>(new OkObjectResult(response));

    }
    
    

    
}