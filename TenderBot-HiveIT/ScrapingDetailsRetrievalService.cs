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
    


        public List<string> GetPageLinks(string url)
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
        
        // public void GetAllPageDDTga
    
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
                        
                        pageDetails.SetDayRateOrBudget(htmlNode);  

                        var title = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//h1[@class='govuk-heading-l']").InnerText;
                        var department = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//span[@class='govuk-caption-l']").InnerText;
                        var publishedDate = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Published')]/following-sibling::dd").InnerText;
                        var deadline = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Deadline')]/following-sibling::dd").InnerText;
                        var link = "https://www.digitalmarketplace.service.gov.uk" + url;
                        var description = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Summary')]/following-sibling::dd").InnerText;
                        var closing = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Closing')]/following-sibling::dd").InnerText;
                        var location = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Location')]/following-sibling::dd").InnerText;

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
        
        public Details GetNewPageOverviewDetails(String url)
        {

            
                var htmlNode = GetHtml(url);
                var pageDetails = new Details();
                pageDetails.Id = url.Substring(url.Length - 5);
                
                        
                        pageDetails.SetDayRateOrBudget(htmlNode);  

                        var title = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//h1[@class='govuk-heading-l']").InnerText;
                        var department = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//span[@class='govuk-caption-l']").InnerText;
                        var publishedDate = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Published')]/following-sibling::dd").InnerText;
                        var deadline = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Deadline')]/following-sibling::dd").InnerText;
                        var description = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Summary')]/following-sibling::dd").InnerText;
                        var closing = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Closing')]/following-sibling::dd").InnerText;
                        var location = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Location')]/following-sibling::dd").InnerText;

                        pageDetails.SetValues(title, department, publishedDate, deadline, url, description, closing, location);
                        
                
                
            
            return pageDetails;

        }

        public HtmlNode GetHtml(string url)
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

    public MoreDetails GetMoreInformationObject(String url)
    {
        var htmlNode = GetHtml(url);
        var moreDetails = new MoreDetails();

        if (htmlNode.OwnerDocument.DocumentNode
                .SelectSingleNode("//dt[contains(text(), 'Why the work is being done')]/following-sibling::dd")
                .InnerText != null)
        {
            moreDetails.WhyTheWorkIsBeingDone = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Why the work is being done')]/following-sibling::dd").InnerText;

        }

        if (htmlNode.OwnerDocument.DocumentNode
                .SelectSingleNode("//dt[contains(text(), 'Who the users are and what they need to do')]/following-sibling::dd")
                .InnerText != null)
        {
            moreDetails.UsersAndWhatTheyNeedToDo = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Who the users are and what they need to do')]/following-sibling::dd").InnerText;

        }
        
        if (htmlNode.OwnerDocument.DocumentNode
                     .SelectSingleNode("//dt[contains(text(), 'Any work that’s already been done')]/following-sibling::dd")
                     .InnerText != null)
        {
            moreDetails.WorkThatsAlreadyBeenDone = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Any work that’s already been done')]/following-sibling::dd").InnerText;
        }

        return moreDetails;

    }

    public string getSpecificData(string dataWanted)
    {
        return "test";
    }
    
    
    
    
    
}