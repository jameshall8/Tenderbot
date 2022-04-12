using System;
using CsvHelper;
using HtmlAgilityPack;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

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
            return pageLinks;
        }

        static List<Details> GetPageDetails(List<string> urls){
            var lstpageDetails = new List<Details>();

            foreach (var url in urls){
                var HtmlNode = GetHtml("https://www.digitalmarketplace.service.gov.uk" + url);
                var pageDetails = new Details();

                pageDetails.Title = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//html/head/title").InnerText;
                var description = HtmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//html/body/dd").InnerText;

            }
            return lstpageDetails;

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
