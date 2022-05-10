using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TenderBot_HiveIT;

namespace HiveIT.Tenderbot;

public static class SlackInteractivity
{
    [FunctionName("SlackInteractivity")]
    public static void RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequest req, ILogger log)
    {
        var messagingService = new SlackMessagingService();

        try
        {
            var payload = req.Form["payload"];

            string url;

            var tableService = new TableStorageDatabaseService();
            var scrapingService = new ScrapingDetailsRetrievalService(tableService);
            var payloadService = new PayloadFormattingService();

            string text = payloadService.GetText(payload).Trim();

            if (text == "Send To Favourites")
            {
                string username = payloadService.GetUsername(payload);
                string id = payloadService.GetId(payload).Trim();
                if (tableService.CheckIfNew(id) == false)
                {
                    url = tableService.GetUrlForMoreInfo(id);
                    Details details = scrapingService.GetNewPageOverviewDetails(url);
                    string name = payloadService.GetUsername(payload);
                    messagingService.SendToTenderbotSlack(details, true, name);

                }
            }
            else if (text == "select")
            {
                var selected = payloadService.GetSelected(payload);
                string id = payloadService.GetSelectedId(payload);

                url = tableService.GetUrlForMoreInfo(id);

                if (url != null) messagingService.PostSelectedDataToSlack(scrapingService, selected, url);
            }
        }
        catch (NullReferenceException e)
        {
            messagingService.PostObjectErrorMessage(e);
        }
        catch (UriFormatException uri)
        {
            messagingService.PostObjectErrorMessage(uri);
        }

    }




}