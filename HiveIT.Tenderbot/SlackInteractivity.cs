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
using TenderBot_HiveIT;

namespace HiveIT.Tenderbot;

public static class SlackInteractivity
{
    [FunctionName("SlackInteractivity")]
    public static Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        var payload = req.Form["payload"];

        var messagingService = new SlackMessagingService();

        string id = messagingService.GetId(payload).Trim();
        string text = messagingService.GetText(payload).Trim();

        if (text == "More Info")
        {
            messagingService.SendMoreInfoToSlack(new MoreDetails(), id);   
        }
        else
        {
            var tableService = new TableStorageDatabaseService();

        
            if (tableService.CheckIfNew(id) == false)
            {
                var url = tableService.GetUrlForMoreInfo(id);
                var scrapingService = new ScrapingDetailsRetrievalService(tableService);

                Details details = scrapingService.GetNewPageOverviewDetails(url);

                SlackMessagingService slackService = new SlackMessagingService();
            
                slackService.SendToTenderbotSlack(details);
                
            }
        }  
        SlackResponse response = new SlackResponse()
        {
            ResponseType = "in_channel",
            Text = "Done",
        };
        return Task.FromResult<IActionResult>(new OkObjectResult(response));

    }
    
    

    
}