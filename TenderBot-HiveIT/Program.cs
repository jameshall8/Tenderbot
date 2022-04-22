using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using Azure.Data.Tables;
using Azure;





namespace TenderBot_HiveIT
{
    public class Program
    {

        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();


        static void Main(string[] args)
        {
            RunScraper();
            //comment
        }

        public static void RunScraper()
        {
            //getting the individual links of the pages 
            var links = GetPageLinks("https://www.digitalmarketplace.service.gov.uk/digital-outcomes-and-specialists/opportunities?statusOpenClosed=open&lot=digital-outcomes");
            //if link is digital outcomes then it returns 4 links not needed, if its digital outcomes and specialists then it only returns 3 not needed.

            //returns a list of Details objects 


            var details = GetPageDetails(links);
            //loop through all the objects and send each to slack via web hook
            foreach (Details detail in details)
            {
                detail.SendToSlack(detail);
            }
        }

        static List<string> GetPageLinks(string url)
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

        static List<Details> GetPageDetails(List<string> urls)
        {
            var listpageDetails = new List<Details>();

            foreach (var url in urls)
            {
                var htmlNode = GetHtml("https://www.digitalmarketplace.service.gov.uk" + url);
                var pageDetails = new Details();
                pageDetails.Id = url.Substring(url.Length - 5);
                if (CheckIfNew(pageDetails.Id))
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

                        pageDetails.AddToDb();
                        listpageDetails.Add(pageDetails);

                }
                else
                {
                    break;
                }
            }
            return listpageDetails;

        }

        public static List<string> GetRidOfNull(List<string> list){
                        
            foreach (string url in list.ToArray()){
                char c = url[url.Length-1];

                if (!Char.IsDigit(c)){
                    list.Remove(url);
                }
            }
            return list;
        }

        public static bool CheckIfNew(string pageid)
        {

            var cs = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);

            TableClient client = new TableClient(cs, "Tenders");

            Pageable<TableEntity> entities = client.Query<TableEntity>(filter: $"ID eq '{pageid}'");

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

        static HtmlNode GetHtml(string url)
        {
            WebPage webPage = _scrapingBrowser.NavigateToPage(new Uri(url));
            return webPage.Html;

        }


    }

}


