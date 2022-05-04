namespace TenderBot_HiveIT;

public class MoreDetails
{
    public string? WhyTheWorkIsBeingDone { get; set; }
    public string? UsersAndWhatTheyNeedToDo { get; set; }

    public string? WorkThatsAlreadyBeenDone { get; set; }



    public void AssignValues(string whyTheWorkIsBeingDone, string problemToBeSolved, string usersAndWhatTheyNeedToDo,
        string workThatsAlreadyBeenDone, string essentialSkills)
    {
        WhyTheWorkIsBeingDone = whyTheWorkIsBeingDone;
        UsersAndWhatTheyNeedToDo = usersAndWhatTheyNeedToDo;
        WorkThatsAlreadyBeenDone = workThatsAlreadyBeenDone;
    }

}