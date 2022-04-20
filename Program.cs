using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using Azure.Data.Tables;
using Azure;


namespace TenderBotGit
{
    class Program
    {

        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();


        static void Main(string[] args)
        {
            runScraper();
        }

        static void runScraper()
        {
            //getting the individual links of the pages 
            var links = GetPageLinks("https://www.digitalmarketplace.service.gov.uk/digital-outcomes-and-specialists/opportunities?statusOpenClosed=open&lot=digital-outcomes");
            //if link is digital outcomes then it returns 4 links not needed, if its digital outcomes and specialists then it only returns 3 not needed.

            //returns a list of Details objects 


            var Details = GetPageDetails(links);
            //loop through all the objects and send each to slack via web hook
            foreach (Details detail in Details)
            {
                detail.sendToSlack(detail);
            }
        }

        static List<string> GetPageLinks(string url)
        {
            var pageLinks = new List<string>();
            var html = GetHtml(url);

            var Links = html.CssSelect("a");
            foreach (var link in Links)
            {
                if (link.Attributes["href"].Value.Contains("opportunities"))
                {
                    pageLinks.Add(link.Attributes["href"].Value);
                }
            }
            return GetRidOfNullURLs(pageLinks);
        }

        static List<Details> GetPageDetails(List<string> urls)
        {
            var listpageDetails = new List<Details>();

            foreach (var url in urls)
            {
                var HtmlNode = GetHtml("https://www.digitalmarketplace.service.gov.uk" + url);
                var pageDetails = new Details();
                pageDetails.ID = url.Substring(url.Length - 5);
                var test = pageDetails.ID;
                if (checkIfNew(pageDetails.ID))
                {
                        
                        pageDetails.setDayRateOrBudget(HtmlNode);  

                        var Title = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//h1[@class='govuk-heading-l']").InnerText;
                        var Department = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//span[@class='govuk-caption-l']").InnerText;
                        var PublishedDate = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Published')]/following-sibling::dd").InnerText;
                        var Deadline = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Deadline')]/following-sibling::dd").InnerText;
                        var Link = "https://www.digitalmarketplace.service.gov.uk" + url;
                        var Description = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Summary')]/following-sibling::dd").InnerText;
                        var Closing = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Closing')]/following-sibling::dd").InnerText;
                        var Location = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Location')]/following-sibling::dd").InnerText;

                        pageDetails.setValues(Title, Department, PublishedDate, Deadline, Link, Description, Closing, Location);

                        pageDetails.addToDB();
                        listpageDetails.Add(pageDetails);

                    }
                else
                {
                    break;
                }
            }
            return listpageDetails;

        }

        static List<string> GetRidOfNullURLs(List<string> List){
                        
            foreach (string URL in List.ToArray()){
                char c = URL[URL.Length-1];

                if (!Char.IsDigit(c)){
                    List.Remove(URL);
                }
            }
            return List;
        }

        static bool checkIfNew(string pageID)
        {
            TableClient client = new TableClient("AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;", "Tenders");

            Pageable<TableEntity> entities = client.Query<TableEntity>(filter: $"ID eq '{pageID}'");

            int counter = 0;

            foreach (TableEntity entity in entities)
            {
                counter = counter + 1;
                if (counter > 0)
                {
                    return false;
                }
            }

            return true;
        }

        static HtmlNode GetHtml(string URL)
        {
            WebPage webPage = _scrapingBrowser.NavigateToPage(new Uri(URL));
            return webPage.Html;

        }


    }

}


