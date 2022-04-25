using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TenderBot_HiveIT;

namespace HiveIT.Tenderbot;

public static class TenderBotTimerTrigger
{
    [FunctionName("TenderBotTimerTrigger")]
    public static async Task RunAsync([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer, ILogger log)
    {
        
        var tableService = new TableStorageDatabaseService();
        var scrapingService = new ScrapingDetailsRetrievalService(tableService);
        var messagingService = new SlackMessagingService();
        
        var program = new Program(tableService,scrapingService,messagingService);


        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        program.RunScraper();


    }
}