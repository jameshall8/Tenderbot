using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TenderBot_HiveIT;

namespace TenderBotTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CheckIfNewId()
    {
        Assert.True(Program.CheckIfNew("12345")); //passing in a new ID
    }

    
    [Test]
    public void CheckIfAlreadyInDatabase()
    {
        Assert.False(Program.CheckIfNew("16883"));
    }

    [Test]
    public void CheckLinksWithoutAnIdAreNotReturned()
    {
        var urlList = new List<string>();
        urlList.Add("www.test@test.com/12345");
        urlList.Add("www.test@test.com");

        Assert.That(Program.GetRidOfNull(urlList), Has.Exactly(1).Items.EqualTo("www.test@test.com/12345"));

        urlList.Add("www.test@test.com/123456");
        urlList.Add("www.test@test.com/123444");
        urlList.Add("www.test@test.com/test");

        Assert.That(Program.GetRidOfNull(urlList), Has.Exactly(3).Items);
    }

    [Test]
    public void CheckDayRate(){

        {
            var details = new Details();
            Assert.AreEqual(details.checkIfDayRate(true), "Budget Range");
            Assert.AreEqual(details.checkIfDayRate(false), "Maximum Day Rate");

        }







}
    
    
}