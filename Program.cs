using System;
using CsvHelper;
using HtmlAgilityPack;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.Text.RegularExpressions;
using Slack.Webhooks;
using System.Data.SqlClient;
using Azure.Storage.Blobs;





namespace TenderBotGit
{
    class Program
    {

        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();

        static void Main(string[] args)
        {


            checkIfNew("17200");
            checkIfNew("50000");




            //getting the individual links of the pages 
            var links = GetPageLinks("https://www.digitalmarketplace.service.gov.uk/digital-outcomes-and-specialists/opportunities?q=&statusOpenClosed=open");


            //returns a list of Details objects 
            var Details = GetPageDetails(links);


            //loop through all the objects and send each to slack via web hook
            foreach (Details detail in Details){
            //    sendToSlack(detail);

            }


        }

        static List<string> GetPageLinks(string url){
            var pageLinks = new List<string>();
            var html = GetHtml(url);

            var Links = html.CssSelect("a");
            foreach (var link in Links){
                if (link.Attributes["href"].Value.Contains("opportunities")){
                    pageLinks.Add(link.Attributes["href"].Value);
                }
            }
            pageLinks.RemoveRange(0, Math.Min(3, pageLinks.Count));
            return pageLinks;
        }

        static List<Details> GetPageDetails(List<string> urls){
            var listpageDetails = new List<Details>();

            foreach (var url in urls){
                var HtmlNode = GetHtml("https://www.digitalmarketplace.service.gov.uk" + url);
                var pageDetails = new Details();

                pageDetails.ID = url.Substring(url.Length - 5);

                

                if (checkIfNew(pageDetails.ID)){


                 pageDetails.Title = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//h1[@class='govuk-heading-l']").InnerText;
                 pageDetails.Department = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//span[@class='govuk-caption-l']").InnerText;

                 pageDetails.PublishedDate = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Published')]/following-sibling::dd").InnerText;
                 pageDetails.Deadline = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Deadline')]/following-sibling::dd").InnerText;
                 pageDetails.Link = "https://www.digitalmarketplace.service.gov.uk" + url;
                 pageDetails.Description = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Summary')]/following-sibling::dd").InnerText;
                 pageDetails.Closing = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Closing')]/following-sibling::dd").InnerText;
                 pageDetails.Location = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Location')]/following-sibling::dd").InnerText;
                 

                 
                 listpageDetails.Add(pageDetails);



                }


            }
            return listpageDetails;

        }

    static void sendToSlack(Details details){
            var slackClient = new SlackClient("https://hooks.slack.com/services/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");
            
            var slackMessage = new SlackMessage
            {
                        Channel = "#leads-tenders",
                        Text = "New Tender Opportunity Posted",
                        IconEmoji = Emoji.RobotFace,
                        Username = "TenderBot"

                        
            };

                    var slackAttachment = new SlackAttachment
            {
                Fallback = "New open task [Urgent]: <"+details.Link + "|" + details.Title + ">",
                Text = "<"+details.Link + "|" + details.Title + ">",
                Color = "#D00000",
                Fields =
                    new List<SlackField>
                        {
                            new SlackField
                                {
                                    Title = "Department",
                                    Value = details.Department,
                                },
                            new SlackField
                                {
                                    Title = "Description",
                                    Value = details.Description,
                                },
                                new SlackField
                                {
                                    Title = "Published Date",
                                    Value = details.PublishedDate,
                                },
                                new SlackField
                                {
                                    Title = "Deadline For Asking Questions",
                                    Value = details.Deadline,
                                },
                                new SlackField
                                {
                                    Title = "Closing Date ",
                                    Value = details.Closing,
                                },
                                new SlackField
                                {
                                    Title = "Location ",
                                    Value = details.Location,
                                }
                        }
            };
        slackMessage.Attachments = new List<SlackAttachment> {slackAttachment};

            

            // slackClient.Post(slackMessage);


    }

    static bool checkIfNew(string pageID){

    

    
    String connString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;";
    using (SqlConnection connection = new SqlConnection(connString))
    {
                        SqlCommand checkID= new SqlCommand("SELECT COUNT(*) FROM Tenders WHERE (ID = @ID)" , connection);
                        checkID.Parameters.AddWithValue("@ID", pageID);
                        int UserExist = (int)checkID.ExecuteScalar();

                        if(UserExist > 0)
                        {
                        return false;
                        }
                        else
                        {
                        return true;
                        }

                }
    }

    
    static HtmlNode GetHtml(string URL){
        WebPage webPage = _scrapingBrowser.NavigateToPage(new Uri(URL));
        return webPage.Html;

    }


}
        public class Details{
            public string Title { get; set; }

            public string ID { get; set; }

            public string Department { get; set; }
            
            public string Link { get; set; }

            public string Description { get; set; }

            public string PublishedDate { get; set; }

            public string Deadline { get; set; }

            public string Closing { get; set; }

            public string Location { get; set; }
        }
}
