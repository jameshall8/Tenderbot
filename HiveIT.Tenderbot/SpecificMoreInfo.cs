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

namespace HiveIT.Tenderbot;

public static class SpecificMoreInfo
{
    [FunctionName("SpecificMoreInfo")]
    public static Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        string infoWanted = req.Form["text"].ToString().Trim();
        
        
        
        
        
        
        
        SlackResponse response = new SlackResponse()
        {
            ResponseType = "in_channel",
            // Text = "There is no tender with that ID, Please try command again with new ID."
            Text = "hello"

        };
        
        return Task.FromResult<IActionResult>(new OkObjectResult(response));

        
    }
}