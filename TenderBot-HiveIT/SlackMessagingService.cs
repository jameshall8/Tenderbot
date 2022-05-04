using Slack.Webhooks;
using Peaky.Slack.BlockKit;
using Peaky.Slack.BlockKit.Composition;
using Peaky.Slack.BlockKit.Elements;
using Peaky.Slack.BlockKit.Entities;
using Peaky.Slack.BlockKit.Layout;
using Slack.Webhooks.Blocks;

namespace TenderBot_HiveIT;

public class SlackMessagingService : IMessagingService
{

    public void SendToTenderbotSlack(Details details)
    {
        //hive webhook 

        // var slackClient = new SlackClient("https://hooks.slack.com/services/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");

        //test webhook 
        var slackClient =
            new SlackClient("https://hooks.slack.com/services/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");


        var slackMessage = new SlackMessage
        {
            Channel = "#tenderbot-favourites",
            Text = "New Tender Opportunity Posted",
            IconEmoji = Emoji.RobotFace,
            Username = "TenderBot"
        };
        var slackAttachment = GetAttachment(details);
        slackMessage.Attachments = new List<SlackAttachment> { slackAttachment };
        slackClient.Post(slackMessage);
    }

    public void SendMoreInfoToSlack(MoreDetails details, string id)
    {
        var slackClient =
            new SlackClient("https://hooks.slack.com/services/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");

        var slackMessage = new SlackMessage
        {
            Channel = "#leads-tenders",
            Text = "More Info Below for tender " + id,
            IconEmoji = Emoji.RobotFace,
            Username = "TenderBot"
        };
        var slackAttachment = GetAttachment(details, id);
        slackMessage.Attachments = new List<SlackAttachment> { slackAttachment };
        slackClient.Post(slackMessage);
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
                    new SlackField
                    {
                        Title = "ID",
                        Value = details.Id,
                        Short = true

                    }

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

    public void GetBlockAttachment()
    {

        BlockMessage blockMessage = new BlockMessage()
        {
            Blocks =
            {
                new SectionBlock()
                {
                    BlockId = "block1",
                    Text = new PlainTextComposition()
                    {
                        Text = "hello hello"
                    },
                    Accessory = new ButtonElement()
                    {
                        Text = new PlainTextComposition()
                        {
                            Text = "hello"
                        },
                        Value = "45545"
                    }

                }
            }
        };



    }
}