using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Slack.Webhooks;
using Slack.Webhooks.Blocks;
using Slack.Webhooks.Elements;
using TenderBot_HiveIT;

namespace HiveIT.Tenderbot;

public static class MoreInfo
{
    [FunctionName("MoreInfo")]
    public static Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        HttpRequest req,
        ILogger log)
    {
        var id = req.Form["text"].ToString().Trim();

        var tableService = new TableStorageDatabaseService();

        if (tableService.CheckIfNew(id) == false)
        {
            var url = tableService.GetUrlForMoreInfo(id);
            var ScrapingService = new ScrapingDetailsRetrievalService(tableService);

            MoreDetails moreDetails = ScrapingService.GetMoreInformationObject(url);

            SlackMessagingService slackService = new SlackMessagingService();

            slackService.SendMoreInfoToSlack(moreDetails, id);
        }
        else
        {
            // Root root = new Root()
            // {
            // var Blocks = new List<Block>
            // {
            //     new Block()
            //     {
            //         Type = "divider"
            //     },
            //     new Block()
            //     {
            //         Type = "section",
            //         Text = new Text()
            //         {
            //             Type = "plain_text",
            //             text = "this is a plain text section block",
            //             Emoji = true
            //         }
            //
            //     },
            //     new Block()
            //     {
            //         Type = "section",
            //         Text = new Text()
            //         {
            //             Type = "mrkdwn",
            //             text = "Section block with radio buttons"
            //         },
            //         Accessory = new Accessory()
            //         {
            //             Type = "radio_buttons",
            //             Options = new List<Option>()
            //             {
            //                 new Option()
            //                 {
            //                     Text = new Text()
            //                     {
            //                         Type = "plain_text",
            //                         text = "this is plain text",
            //                         Emoji = true
            //
            //                     },
            //                     Value = "value-0"
            //                 },
            //                 new Option()
            //                 {
            //                     Text = new Text()
            //                     {
            //                         Type = "plain_text",
            //                         text = "this is plain text",
            //                         Emoji = true
            //
            //                     },
            //                     Value = "value-1"
            //                 },
            //                 new Option()
            //                 {
            //                     Text = new Text()
            //                     {
            //                         Type = "plain_text",
            //                         text = "this is plain text",
            //                         Emoji = true
            //
            //                     },
            //                     Value = "value-2"
            //                 }
            //
            //             },
            //             action_id = "radio_buttons-action"
            //         }
            //     }
            // };
            // // };
            
            var slacker =
                new SlackClient("https://hooks.slack.com/services/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");
            
            
            SlackResponse response = new SlackResponse()
            {
                ResponseType = "in_channel",
                Text = "There is no Tender with that ID",
            };

            var slackermes = new SlackMessage()
            {
                Channel = "#bot",
                Text = "New Tender Opportunity Posted",
                IconEmoji = Emoji.RobotFace,
                Username = "TenderBot",
                Blocks = new List<Slack.Webhooks.Block>()
                {
                    new Header()
                    {
                        Text = new TextObject()
                        {
                            Text = "Hello testy boy lol"
                        } 
                    },
                    new Divider(),
                    new Divider(),
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
                            Options = new List<Slack.Webhooks.Elements.Option>()
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
                   


                }
            };
            slacker.Post(slackermes);

            return Task.FromResult<IActionResult>(new OkObjectResult(response));



        }

        // var slackClient =
        //     new SlackClient("https://hooks.slack.com/services/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");


        // var slackAttachment = response.GetAttachment();
        // slackMessage.Attachments = new List<SlackAttachment> { slackAttachment };
        




        SlackResponse delivered = new SlackResponse()
        {
            ResponseType = "in_channel",
            Text = "delivered"
        };

        return Task.FromResult<IActionResult>(new OkObjectResult(delivered));


    }
}


       

    
     
