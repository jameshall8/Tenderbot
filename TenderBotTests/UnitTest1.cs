using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TenderBot_HiveIT;

namespace TenderBotTests;

public class Tests
{
    static IDatabaseService tableService = new TableStorageDatabaseService();
    static IDetailsRetrievalService scrapingService = new ScrapingDetailsRetrievalService(tableService);
    static IMessagingService messagingService = new SlackMessagingService(); 
    
    Program program = new Program(tableService,scrapingService,messagingService);
    
    
    [SetUp]
    public void Setup()
    {
        
    }

    // [Test]
    // public void CheckIfNewId()
    // {
    //     Assert.True(Program.CheckIfNew("12345")); //passing in a new ID
    // }
    //
    //
    // [Test]
    // public void CheckIfAlreadyInDatabase()
    // {
    //     Assert.False(Program.CheckIfNew("16883"));
    // }

    [Test]
    public void CheckLinksWithoutAnIdAreNotReturned()
    {
        var urlList = new List<string>();
        urlList.Add("www.test@test.com/12345");
        urlList.Add("www.test@test.com");

        Assert.That(program.ScrapeService.GetRidOfNull(urlList), Has.Exactly(1).Items.EqualTo("www.test@test.com/12345"));

        urlList.Add("www.test@test.com/123456");
        urlList.Add("www.test@test.com/123444");
        urlList.Add("www.test@test.com/test");

        Assert.That(program.ScrapeService.GetRidOfNull(urlList), Has.Exactly(3).Items);
    }

    [Test]
    public void CheckDayRate(){

        {
            var details = new Details();
            Assert.AreEqual(details.checkIfDayRate(true), "Budget Range");
            Assert.AreEqual(details.checkIfDayRate(false), "Maximum Day Rate");

        }






    }
    
    [Test]
    public void GetPageLinks_NoNewDetailsFound()
    {
        
        var mockDatabaseService = new Mock<IDatabaseService>();
        var mockScraperService = new Mock<IDetailsRetrievalService>();
        var mockSlackService = new Mock<IMessagingService>();
        
        mockDatabaseService
            .Setup(service => service.CheckIfNew(It.IsAny<string>()))
            .Returns(false);
        
        var pro = new Program(mockDatabaseService.Object, mockScraperService.Object, mockSlackService.Object);
        Assert.IsEmpty(program.ScrapeService.GetPageLinks("https://www.digitalmarketplace.service.gov.uk/digital-outcomes-and-specialists/opportunities?statusOpenClosed=open&lot=digital-outcomes"));
    }
    
    [Test]
    public void RunScraper_AllNewDetailsFound()
    {
        var mockDatabaseService = new Mock<IDatabaseService>();
        var mockScraperService = new Mock<IDetailsRetrievalService>();
        var mockSlackService = new Mock<IMessagingService>();

        
        mockDatabaseService
            .Setup(service => service.CheckIfNew(It.IsAny<string>()))
            .Returns(false);
        
        
        
        var pro = new Program(mockDatabaseService.Object, mockScraperService.Object, mockSlackService.Object);
        Assert.That(program.ScrapeService.GetPageLinks("https://www.digitalmarketplace.service.gov.uk/digital-outcomes-and-specialists/opportunities?statusOpenClosed=open&lot=digital-outcomes"), Has.Exactly(18).Items);
    }
    
    
}