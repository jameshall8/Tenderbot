using Slack.Webhooks;
using System.Collections.Generic;

public class Details
    {
        public string Title { get; set; }

        public bool BudgetOrDayRate { get; set; } //false == day rate 

        public string Budget { get; set; }

        public string ID { get; set; }

        public string Department { get; set; }

        public string Link { get; set; }

        public string Description { get; set; }

        public string PublishedDate { get; set; }

        public string Deadline { get; set; }

        public string Closing { get; set; }

        public string Location { get; set; }

         public void sendToSlack(Details details)
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


        string checkIfDayRate(bool check)
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


    }