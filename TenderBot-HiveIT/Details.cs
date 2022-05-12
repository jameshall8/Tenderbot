using System.Text.RegularExpressions;
using Slack.Webhooks;
using Azure.Data.Tables;
using HtmlAgilityPack;
using TenderBot_HiveIT;


public class Details
    {
        public string? Title { get; set; }

        public bool BudgetOrDayRate { get; set; } //false == day rate 

        public string? Budget { get; set; }

        public string? Id { get; set; }

        public string? Department { get; set; }

        public string? Link { get; set; }

        public string? Description { get; set; }

        public string? PublishedDate { get; set; }

        public string? Deadline { get; set; }

        public string? Closing { get; set; }

        public string? Location { get; set; }


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

        

        public void SetValues(string title, string department, string publishedDate,string deadline,string? link,string description,string closing,string location){
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