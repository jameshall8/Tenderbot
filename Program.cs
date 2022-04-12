using System;
using CsvHelper;
using HtmlAgilityPack;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System.Text.RegularExpressions;



namespace TenderBotGit
{
    class Program
    {

        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();

        static void Main(string[] args)
        {

            var links = GetPageLinks("https://www.digitalmarketplace.service.gov.uk/digital-outcomes-and-specialists/opportunities?q=&statusOpenClosed=open");
            GetPageDetails(links);
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
            var lstpageDetails = new List<Details>();

            foreach (var url in urls){
                var HtmlNode = GetHtml("https://www.digitalmarketplace.service.gov.uk" + url);
                var pageDetails = new Details();

                string page_ID = url.Substring(url.Length - 5);

                

                if (checkIfNew(page_ID)){


                 pageDetails.Title = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//h1[@class='govuk-heading-l']").InnerText;
                 pageDetails.PublishedDate = formatData(HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//div[@class='govuk-summary-list__row']/dd[@class='govuk-summary-list__value']").InnerText);
                 pageDetails.Deadline = formatData(HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dl[@class='govuk-summary-list app-govuk-summary-list app-govuk-summary-list--top-border govuk-!-margin-bottom-8']/div[@class='govuk-summary-list__row'][2]/dd[@class='govuk-summary-list__value'][1]").InnerText);
                 pageDetails.Link = "https://www.digitalmarketplace.service.gov.uk" + url;
                 pageDetails.Description = formatData(HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dl[@class='govuk-summary-list app-govuk-summary-list app-govuk-summary-list--top-border govuk-!-margin-bottom-8'][2]/div[@class='govuk-summary-list__row'][2]").InnerText, "Summary of the work").Trim();
                 pageDetails.Closing = formatData(HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dl[@class='govuk-summary-list app-govuk-summary-list app-govuk-summary-list--top-border govuk-!-margin-bottom-8']/div[@class='govuk-summary-list__row'][3]/dd[@class='govuk-summary-list__value'][1]").InnerText);
                 pageDetails.Location = formatData(HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dl[@class='govuk-summary-list app-govuk-summary-list app-govuk-summary-list--top-border govuk-!-margin-bottom-8'][2]/div[@class='govuk-summary-list__row'][5]").InnerText, "Location");

                

                 var test = pageDetails.Closing;




                }


            }
            return lstpageDetails;

        }

    static bool checkIfNew(string pageID){
        return true;
    }

    static string formatData(string input, string wordToRemove){
        // input = Regex.Replace(input, @"[ \t]+(\r?$)", string.Empty);
        input = input.Trim();
        input = input.Replace(wordToRemove, "");

        return input;
    }

    static string formatData(string input){
        // input = Regex.Replace(input, @"[ \t]+(\r?$)", string.Empty);
        input = input.Trim();

        return input;
    }
    static HtmlNode GetHtml(string URL){
        WebPage webPage = _scrapingBrowser.NavigateToPage(new Uri(URL));
        return webPage.Html;

    }


}
        public class Details{
            public string Title { get; set; }
            
            public string Link { get; set; }

            public string Description { get; set; }

            public string PublishedDate { get; set; }

            public string Deadline { get; set; }

            public string Closing { get; set; }

            public string Location { get; set; }
        }
}
