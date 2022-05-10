using Newtonsoft.Json.Linq;
using Slack.Webhooks;
using Slack.Webhooks.Blocks;
using Slack.Webhooks.Elements;

namespace TenderBot_HiveIT;

public class SlackMessagingService : IMessagingService
{

    public SlackClient GetSlackClient()
    {
        //test webhook 
        var slackClient =
            new SlackClient("https://hooks.slack.com/services/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");

        return slackClient;
    }

    public void SendToTenderbotSlack(Details details)
    {
        //hive webhook 

        // var slackClient = new SlackClient("https://hooks.slack.com/services/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");

        //test webhook 
        var slackClient = GetSlackClient();

        var message = GetSlackMessageTemplate();

        var slackAttachment = GetAttachment(details);
        message.Attachments = new List<SlackAttachment> { slackAttachment };
        slackClient.Post(message);
        SendTrailMessage(details, slackClient);
    }

    private void SendTrailMessage(Details details, SlackClient client)
    {
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
            Channel = "#tenderbot-favourites",
            Text = "New Tender Opportunity Posted",
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
            Text = "New Tender Opportunity Posted",
            IconEmoji = Emoji.RobotFace,
            Username = "TenderBot",
        };
        return slackMessage;
    }

    public void SendMoreInfoToSlack(MoreDetails details, string id)
    {
        var tableService = new TableStorageDatabaseService();

        if (tableService.CheckIfNew(id) == false) //checking that the ID is within the DB
        {
            var url = tableService.GetUrlForMoreInfo(id);
            var scrapingService = new ScrapingDetailsRetrievalService(tableService);

            MoreDetails moreDetails = scrapingService.GetMoreInformationObject(url);

            SlackMessagingService slackService = new SlackMessagingService();

            var attachment = slackService.GetAttachment(moreDetails, id);

            SlackMessage message = GetSlackMoreInfoMessageTemplate();
            
            message.Attachments = new List<SlackAttachment> { attachment };
            
            var slackClient = GetSlackClient();

            slackClient.Post(message);
        }
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

    private SlackAttachment GetAttachment(MoreDetails details, string id)
    {
        var slackAttachment = new SlackAttachment
        {
            Fallback = id,
            Color = "#0b0c0c",
            Fields =
                new List<SlackField>
                {
                    new SlackField
                    {

                        Title = "Why The Work Is Being Done",
                        Value = details.WhyTheWorkIsBeingDone,
                    },
                    new SlackField
                    {
                        Title = "Who the users are and what they need to do",
                        Value = details.UsersAndWhatTheyNeedToDo,

                    },
                    new SlackField
                    {
                        Title = "Any work that’s already been done",
                        Value = details.WorkThatsAlreadyBeenDone,

                    },
                },
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
                Text = "This button will post more information",
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
                    Text = "More Info",
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
                            Text = "Select an item",
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

    public List<Block> GetDropDownBlockForMoreInfo()
    {
        var blocks = new List<Block>()
        {
            new Header()
            {
                Text = new TextObject()
                {
                    Text = "Hello testy boy lol"
                }
            },
            new Divider(),
            new Section()
            {
                Text = new TextObject()
                {
                    Text = "Pick an item from the dropdown",
                    Type = TextObject.TextType.Markdown
                },

                Accessory = new SelectStatic()
                {
                    ActionId = "radio_buttons-action",
                    Placeholder = new TextObject()
                    {
                        Type = TextObject.TextType.PlainText,
                        Text = "select an item",
                        Emoji = true
                    },
                    Options = new List<Option>()
                    {
                        new()
                        {
                            Text = new TextObject()
                            {
                                Type = TextObject.TextType.PlainText,
                                Text = "this is plain text",
                                Emoji = true
                            }
                        },
                        new()
                        {
                            Text = new TextObject()
                            {
                                Type = TextObject.TextType.PlainText,
                                Text = "this is plain text",
                                Emoji = true
                            }
                        },
                        new()
                        {
                            Text = new TextObject()
                            {
                                Type = TextObject.TextType.PlainText,
                                Text = "this is plain text",
                                Emoji = true
                            }
                        },
                    }
                }

            }
        };
        return blocks;
    }
    public string GetId(string payload)
    {

        JObject obj = JObject.Parse(payload);
        var actions = obj["actions"];
        
        var actionsKeyValue = actions[0];
        
        var id = actionsKeyValue["value"].ToString();
        
        return id;
    }
    
    public string GetText(string payload)
    {
        try
        {
            JObject obj = JObject.Parse(payload);
            var actions = obj["actions"];

            var actionsKeyValue = actions[0];

            var textArray = actionsKeyValue["text"].ToString();


            obj = JObject.Parse(textArray);

            var text = obj["text"].ToString();

            return text;
        }
        catch
        {
            return "select";
        }
        
    }

    public List<string> GetSelected(string payload)
    {
        List<string> selectedText = new List<string>();
        JObject obj = JObject.Parse(payload);
        var actions = obj["actions"];

        var actionsKeyValue = actions[0];

        var textArray = actionsKeyValue["selected_options"].ToArray();

        foreach (var token in textArray)
        {
            obj = JObject.Parse(token.ToString());
            var textString = obj["text"];
            var finalText = textString["text"].ToString().Trim();
            selectedText.Add(finalText);
        }

        return selectedText;
    }
    
}