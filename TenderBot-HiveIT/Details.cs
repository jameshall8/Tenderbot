using Slack.Webhooks;
using System.Collections.Generic;
using Azure.Data.Tables;
using HtmlAgilityPack;



public class Details
    {
        public string Title { get; set; }

        public bool BudgetOrDayRate { get; set; } //false == day rate 

        public string Budget { get; set; }

        public string Id { get; set; }

        public string Department { get; set; }

        public string Link { get; set; }

        public string Description { get; set; }

        public string PublishedDate { get; set; }

        public string Deadline { get; set; }

        public string Closing { get; set; }

        public string Location { get; set; }

         public void SendToSlack(Details details)
        {
            var slackClient = new SlackClient("https://hooks.slack.com/services/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");

            var slackMessage = new SlackMessage
            {
                Channel = "#leads-tenders",
                Text = "New Tender Opportunity Posted",
                IconEmoji = Emoji.RobotFace,
                Username = "TenderBot",
            };
            var slackAttachment = GetAttachment(details);
            slackMessage.Attachments = new List<SlackAttachment> { slackAttachment };
            slackClient.Post(slackMessage);
        }


        SlackAttachment GetAttachment(Details details){
            var slackAttachment = new SlackAttachment
            {
                Fallback = details.Title,
                Color = "#0b0c0c",
                AuthorName = details.Department,
                AuthorLink = "//www.digitalmarketplace.service.gov.uk/digital-outcomes-and-specialists/opportunities",
                Title = details.Title,
                TitleLink = details.Link,
                Text = details.Description,
                Fields =
            new List<SlackField>
                {
                                new SlackField
                                {

                                    Title = checkIfDayRate(details.BudgetOrDayRate),
                                    Value = details.Budget,
                                },
                                new SlackField
                                {
                                    Title = "Closing Date ",
                                    Value = details.Closing,
                                    Short = true

                                },
                                new SlackField
                                {
                                    Title = "Location ",
                                    Value = details.Location,
                                    Short = true

                                }
                },
                ThumbUrl = details.Link,
            };

            return slackAttachment;
        }


        public string checkIfDayRate(bool check)
        {
            if (check)
            {
                return "Budget Range";
            }
            else
            {
                return "Maximum Day Rate";
            }

        }

        public void AddToDb()
        {
            
            var cs = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);

            TableClient client = new TableClient(cs, "Tenders");

            var entity = new TableEntity("Tenders", Id)
        {
            { "ID", Id },
            { "Title", Title },
            { "Department", Department },
            { "Link", Link },
            { "Description", Description },
            { "PublishedDate", PublishedDate },
            { "Deadline", Deadline },
            { "Closing", Closing },
            { "Location", Location }
        };
            client.AddEntity(entity);
        }


        public void SetDayRateOrBudget(HtmlNode htmlNode){
        
                if (htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Budget')]/following-sibling::dd").InnerText == null){
                    BudgetOrDayRate = false;
                    Budget = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Maximum day')]/following-sibling::dd").InnerText;
                }
                else {
                    Budget = htmlNode.OwnerDocument.DocumentNode.SelectSingleNode("//dt[contains(text(), 'Budget')]/following-sibling::dd").InnerText;
                    BudgetOrDayRate = true;
                }

                if (Budget == "")
                {
                    Budget = "Did not specify budget";
                }                            
        }

        public void SetValues(string title, string department, string publishedDate,string deadline,string link,string description,string closing,string location){
            Title = title;
            Department = department;
            PublishedDate = publishedDate;
            Deadline = deadline; 
            Link = link;
            Description = description;
            Closing = closing;
            Location = location;
        }


    }