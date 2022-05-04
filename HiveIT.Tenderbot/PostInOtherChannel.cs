using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TenderBot_HiveIT;

namespace HiveIT.Tenderbot;

public static class PostInOtherChannel
{
    [FunctionName("PostInOtherChannel")]
    public static Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {

        var id = req.Form["text"].ToString();
        
        var tableService = new TableStorageDatabaseService();

        
        if (tableService.CheckIfNew(id) == false)
        {
            var url = tableService.GetUrlForMoreInfo(id);
            var ScrapingService = new ScrapingDetailsRetrievalService(tableService);

            Details details = ScrapingService.GetNewPageOverviewDetails(url);

            SlackMessagingService slackService = new SlackMessagingService();
            
            slackService.SendToTenderbotSlack(details);

            return Task.FromResult<IActionResult>(new OkObjectResult(new SlackResponse()
            {
                ResponseType = "in_channel",
                Text = "Tender Posted In Favorites Channel"
            }));
        }
        else
        {
            return Task.FromResult<IActionResult>(new OkObjectResult(new SlackResponse()
            {
                ResponseType = "in_channel",
                Text = "ID does not exist"
            }));

        }
        
        
        
        
        
    }
}