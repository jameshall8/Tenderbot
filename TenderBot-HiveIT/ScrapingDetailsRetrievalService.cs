using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace TenderBot_HiveIT;

public class ScrapingDetailsRetrievalService : IDetailsRetrievalService 
{
    
    IDatabaseService databaseService;
    
    public ScrapingDetailsRetrievalService(IDatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }
    


        public List<string> GetPageLinks(string? url)
    {
        var pageLinks = new List<string>();
        var html = GetHtml(url);

        var links = html.CssSelect("a");
        foreach (var link in links)
        {
            if (link.Attributes["href"].Value.Contains("opportunities"))
            {
                pageLinks.Add(link.Attributes["href"].Value);
            }
        }
        return GetRidOfNull(pageLinks);
    }

        public List<String> GetAllPageDtTagsFromPage(string? url)
        {
            var dlTags = new List<string>();
            var html = GetHtml(url);

            var tags = html.CssSelect("dt");
            foreach (var tag in tags)
            {
                dlTags.Add(tag.InnerHtml.Trim());
            }

            return dlTags;
        }
    
        public List<Details> GetNewPageOverviewDetails(List<string> urls)
        {
            var listpageDetails = new List<Details>();

            foreach (var url in urls)
            {
                var htmlNode = GetHtml("https://www.digitalmarketplace.service.gov.uk" + url);
                var pageDetails = new Details();
                pageDetails.Id = url.Substring(url.Length - 5);
                if (databaseService.CheckIfNew(pageDetails.Id))
                {
                        

                        pageDetails = SetDayRateOrBudget(pageDetails, htmlNode);

                        var title = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//h1[@class='govuk-heading-l']").InnerText;
                        var department = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//span[@class='govuk-caption-l']").InnerText;
                        var link = "https://www.digitalmarketplace.service.gov.uk" + url;
                        var publishedDate = GetTextFromSite("Published", htmlNode);
                        
                        var deadline = GetTextFromSite("Deadline", htmlNode);
                        var description = GetTextFromSite("Summary", htmlNode);
                        var closing = GetTextFromSite("Closing", htmlNode);
                        var location = GetTextFromSite("Location", htmlNode);
                        
                        pageDetails.SetValues(title, department, publishedDate, deadline, link, description, closing, location);

                        databaseService.StoreDetails(pageDetails);
                        listpageDetails.Add(pageDetails);

                }
                else
                {
                    break;
                }
            }
            return listpageDetails;

        }

        string GetTextFromSite(string neededData, HtmlNode htmlNode)
        {
            string text =  htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), '" + neededData + "')]/following-sibling::dd").InnerText;

            text = Decode(text);

            return text;
            
            
        }
        
        public Details GetNewPageOverviewDetails(string? url)
        {

            
                var htmlNode = GetHtml(url);
                var pageDetails = new Details();
                pageDetails.Id = url.Substring(url.Length - 5);
                
                        pageDetails = SetDayRateOrBudget(pageDetails, htmlNode);

                        var title = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//h1[@class='govuk-heading-l']").InnerText;
                        var department = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//span[@class='govuk-caption-l']").InnerText;
                        var publishedDate = GetTextFromSite("Published", htmlNode);
                        
                        var deadline = GetTextFromSite("Deadline", htmlNode);
                        var description = GetTextFromSite("Summary", htmlNode);
                        var closing = GetTextFromSite("Closing", htmlNode);
                        var location = GetTextFromSite("Location", htmlNode);
                        
                        
                        pageDetails.SetValues(title, department, publishedDate, deadline, url, description, closing, location);
                        
                
                
            
            return pageDetails;

        }

        public HtmlNode GetHtml(string? url)
    {
        ScrapingBrowser ScrapingBrowser = new ScrapingBrowser();

        WebPage webPage = ScrapingBrowser.NavigateToPage(new Uri(url));
        return webPage.Html;

    }
    
    public List<string> GetRidOfNull(List<string> list){
                        
        foreach (string url in list.ToArray()){
            char c = url[url.Length-1];

            if (!Char.IsDigit(c)){
                list.Remove(url);
            }
        }
        return list;
    }

    public String GetSelectedData(string tag, string url)
    {

        var htmlNode = GetHtml(url);

        string text  = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), '"+ tag + "')]/following-sibling::dd").InnerText;

        text = Decode(text);
        return text;
    }
    
    public string Decode(String text)
    {
        string formattedString = HttpUtility.HtmlDecode(text);

        return formattedString;
    }

    private Details SetDayRateOrBudget(Details pagedetails, HtmlNode htmlNode){
        
        if (htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Budget')]/following-sibling::dd").InnerText == null){
            
            pagedetails.BudgetOrDayRate = false;
            pagedetails.Budget = GetTextFromSite("Maximum day", htmlNode);
        }
        else {
            
            pagedetails.Budget = GetTextFromSite("Budget", htmlNode);
            pagedetails.BudgetOrDayRate = true;
        }

        if (Regex.Matches(pagedetails.Budget,@"[a-zA-Z]").Count < 1)
        {
            pagedetails.Budget = "Did not specify budget";
        }

        return pagedetails;
    }
    
    
    
    
}