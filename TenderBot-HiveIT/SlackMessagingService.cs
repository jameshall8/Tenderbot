using Slack.Webhooks;
using Slack.Webhooks.Blocks;
using Slack.Webhooks.Elements;

namespace TenderBot_HiveIT;

public class SlackMessagingService : IMessagingService
{

    public SlackClient GetSlackFavoriteClient()
    {
        //test webhook 
        var slackClient =
            new SlackClient("https://hooks.slack.com/services/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");

        return slackClient;
    }
    
    public SlackClient GetSlackLeadTendersClient()
    {
        //test webhook 
        var slackClient =
            new SlackClient("https://hooks.slack.com/services/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");

        return slackClient;
    }

    public void SendToTenderbotSlack(Details details, bool favorite, string name)
    {
        //hive webhook 

        // var slackClient = new SlackClient("https://hooks.slack.com/services/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");

        //test webhook 

        SlackMessage message;
        SlackClient slackClient;
        if (favorite)
        {
            slackClient = GetSlackFavoriteClient();
        }
        else
        {
            slackClient = GetSlackLeadTendersClient();

        }

        message = favorite ? GetSlackFavoritesMessageTemplate(name) : GetSlackMessageTemplate();


        var slackAttachment = GetAttachment(details);
        message.Attachments = new List<SlackAttachment> { slackAttachment };
        slackClient.Post(message);

        if (!favorite)
        {
            SendTrailMessage(details);
        }
    }
    public void SendToTenderbotSlack(Details details, bool favorite)
    {
        //hive webhook 

        // var slackClient = new SlackClient("https://hooks.slack.com/services/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");

        //test webhook 

        SlackMessage message;
        SlackClient slackClient;
        if (favorite)
        {
            slackClient = GetSlackFavoriteClient();
        }
        else
        {
            slackClient = GetSlackLeadTendersClient();

        }

        message = GetSlackMessageTemplate();


        var slackAttachment = GetAttachment(details);
        message.Attachments = new List<SlackAttachment> { slackAttachment };
        slackClient.Post(message);

        if (!favorite)
        {
            SendTrailMessage(details);
        }
    }
    
    private void SendTrailMessage(Details details)
    {
        SlackClient client = GetSlackLeadTendersClient();
        var message = GetSlackMessageTemplate();

        List<String> tags = getDlTags(details.Link);
        
        message.Blocks = GetBlockForTender(details, tags);
        
        client.Post(message);
    }

    private List<String> getDlTags(string? url)
    {
        var tableservice = new TableStorageDatabaseService();
        var scrapingService = new ScrapingDetailsRetrievalService(tableservice);
        List<String> tags = scrapingService.GetAllPageDtTagsFromPage(url);

        return tags;
    }

    
    private SlackMessage GetSlackMessageTemplate()
    {
        var slackMessage = new SlackMessage
        {
            Channel = "#leads-tenders",
            Text = "New Tender Opportunity Posted",
            IconEmoji = Emoji.RobotFace,
            Username = "TenderBot",
        };
        return slackMessage;
    }
    
    private SlackMessage GetSlackObjectErrorMessageTemplate(String message)
    {
        var slackMessage = new SlackMessage
        {
            Channel = "#leads-tenders",
            Text = message,
            IconEmoji = Emoji.RobotFace,
            Username = "TenderBot",
        };
        return slackMessage;
    }

    private SlackMessage GetSlackFavoritesMessageTemplate(string name)
    {
        var slackMessage = new SlackMessage
        {
            Channel = "#tenderbot-favourites",
            Text = "Tender Favorited by " + name,
            IconEmoji = Emoji.RobotFace,
            Username = "TenderBot",
        };
        return slackMessage;
    }

    private SlackMessage GetSlackMoreInfoMessageTemplate()
    {
        var slackMessage = new SlackMessage
        {
            Channel = "#leads-tenders",
            Text = "Requested Info Posted Below",
            IconEmoji = Emoji.RobotFace,
            Username = "TenderBot",
        };
        return slackMessage;
    }





    public SlackAttachment GetAttachment(Details details)
    {
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

                        Title = details.checkIfDayRate(details.BudgetOrDayRate),
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

                    },
                },
            ThumbUrl = details.Link,
        };

        return slackAttachment;
    }
    private List<Block> GetBlockForTender(Details details, List<String> tags)
    {
        var blocks = new List<Block>()
        {
            new Divider(),
            new Header()
            {
                Text = new TextObject()
                {
                    Text = "Actions for this tender"
                }
            },
            
            new Section()
            {
                Text = new TextObject()
                {
                    Text = "This button will post the tender in favourites",
                    Type = TextObject.TextType.Markdown
                },

                Accessory = new Button()
                {
                    Type = ElementType.Button,
                    ActionId = "button-action",
                    Value = details.Id,
                    Text = new TextObject()
                            {
                                Type = TextObject.TextType.PlainText,
                                Text = "Send To Favourites",
                                Emoji = true        
                }       
                }
                },
            
                new Section()
                {
                    Text = new TextObject()
                    {
                        Type = TextObject.TextType.Markdown,
                        Text = "Pick Information To Post"
                    },
                    Accessory = new MultiSelectStatic()
                    {
                        Placeholder = new TextObject()
                        {
                            Type = TextObject.TextType.PlainText,
                            Text = "Select a piece of information",
                            Emoji = true
                        },
                        Options = GetOptionForSelect(tags, details.Id),
                        ActionId = "multi_static_select-action"
                    }
                    
                }
        };
        return blocks;
    }

    private List<Option> GetOptionForSelect(List<string> tags, string? iD)
    {
        List<Option> options = new List<Option>();
        foreach (String tag in tags)
        {
            
            options.Add(new Option()
            {
                Text = new TextObject()
                {
                    Type = TextObject.TextType.PlainText,
                    Text = tag,
                    Emoji = true
                },
                Value = iD
            });
        }

        return options;
    }
    

    public void PostSelectedDataToSlack(ScrapingDetailsRetrievalService scrapingService, List<string> selected, string url)
    {
        SlackMessage message = GetSlackMoreInfoMessageTemplate();
        var attachment = GetSelectedMessageAttachment(selected, scrapingService, url);

        message.Attachments = new List<SlackAttachment>
        {
            attachment
        };
        var client = GetSlackLeadTendersClient();

        client.Post(message);
    }


    private SlackAttachment GetSelectedMessageAttachment(List<string> selected, ScrapingDetailsRetrievalService scraper, string url)
    {
        var slackAttachment = new SlackAttachment
        {
            Text = "Info Requested Below",
            Fallback = "Test",
            Color = "#0b0c0c",
            Fields =
                new List<SlackField>
                {
                    
                },
        };

        foreach (var select in selected)
        {
            slackAttachment.Fields.Add(new SlackField
            {
                Title = select,
                Value = scraper.GetSelectedData(select, url),
            });

        }

        return slackAttachment;
    }

    public void PostObjectErrorMessage(Exception e)
    {
        var message = GetSlackObjectErrorMessageTemplate(e.InnerException is NullReferenceException ? "Error - This tenders webpage no longer exists" : "Error - Tender is not in the Database");
        var client = GetSlackFavoriteClient();

        client.Post(message);

    }
    
    

    


}