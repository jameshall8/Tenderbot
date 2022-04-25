using Azure;
using Azure.Data.Tables;

namespace TenderBot_HiveIT;

public class TableStorageDatabaseService : IDatabaseService
{
    public bool CheckIfNew(string pageid)
    {
        var cs = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);

        TableClient client = new TableClient(cs, "Tenders");

        Pageable<TableEntity> entities = client.Query<TableEntity>(filter: $"ID eq '{pageid}'");

        int counter = 0;

            

        foreach (TableEntity entity in entities)
        {
                
            counter = counter + 1;
            if (counter > 0)
            {
                return false;
            }
        }

        return true;
    }

    public void StoreDetails(Details details)
    {
        var cs = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);

        TableClient client = new TableClient(cs, "Tenders");

        var entity = new TableEntity("Tenders", details.Id)
        {
            { "ID", details.Id },
            { "Title", details.Title },
            { "Department", details.Department },
            { "Link", details.Link },
            { "Description", details.Description },
            { "PublishedDate", details.PublishedDate },
            { "Deadline", details.Deadline },
            { "Closing", details.Closing },
            { "Location", details.Location }
        };
        client.AddEntity(entity);
    }
    
    
}