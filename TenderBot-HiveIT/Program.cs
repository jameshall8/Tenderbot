using System.Runtime.InteropServices;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using Azure.Data.Tables;
using Azure;
using Slack.Webhooks;


namespace TenderBot_HiveIT
{
    
    public interface IDatabaseService
    {
        bool CheckIfNew(string pageid);
        
        void StoreDetails(Details details);
    }
    
    public interface IDetailsRetrievalService
    {
        List<string> GetPageLinks(string? url);

        HtmlNode GetHtml(string? url);

        List<Details> GetNewPageOverviewDetails(List<string> urls);

        List<string> GetRidOfNull(List<string> list);
    }
    
    public interface IMessagingService
    {
        void SendToTenderbotSlack(Details details, bool favorite);
        SlackAttachment GetAttachment(Details details);
    }

    public class Program
    {
        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();
        public IDatabaseService DatabaseService { get; set; }

        public IDetailsRetrievalService ScrapeService { get; set; }

        public IMessagingService SlackMessagingService { get; set; }
        
        public Program(IDatabaseService databaseService, IDetailsRetrievalService scrapeDetailsService, IMessagingService slackService)
        {
             DatabaseService = databaseService;
             ScrapeService = scrapeDetailsService;
             SlackMessagingService = slackService;


        }
        


        static void Main(string[] args)
        {
            
            
            
            var tableService = new TableStorageDatabaseService();
            var scrapingService = new ScrapingDetailsRetrievalService(tableService);
            var messagingService = new SlackMessagingService();
            
            
            new Program(tableService,scrapingService,messagingService).RunScraper();
            
        }

        public void RunScraper()
        {
            //getting the individual links of the pages 
            var links = ScrapeService.GetPageLinks("https://www.digitalmarketplace.service.gov.uk/digital-outcomes-and-specialists/opportunities?statusOpenClosed=open&lot=digital-outcomes");
            //if link is digital outcomes then it returns 4 links not needed, if its digital outcomes and specialists then it only returns 3 not needed.

            //returns a list of Details objects 
            
            var details = ScrapeService.GetNewPageOverviewDetails(links);
            //loop through all the objects and send each to slack via web hook
            foreach (Details detail in details)
            {
                SlackMessagingService.SendToTenderbotSlack(detail, false);
            }
        }

        


    }

}


