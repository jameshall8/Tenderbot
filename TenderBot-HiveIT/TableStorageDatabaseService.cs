using Azure;
using Azure.Data.Tables;

namespace TenderBot_HiveIT;

public class TableStorageDatabaseService : IDatabaseService
{
    public bool CheckIfNew(string pageid)
    {
        string testcs = "AccountName=hiveittenderbotstorage;AccountKey=MhwmuUUF17wEq7noDPOKz06SspZL5IQ8KG0KXzldMnavXPjZLVbOQGAwQbN8FgoWeacwjbOTcHKu+ddQ34YxGw==;BlobEndpoint=https://hiveittenderbotstorage.blob.core.windows.net/;QueueEndpoint=https://hiveittenderbotstorage.queue.core.windows.net/;TableEndpoint=https://hiveittenderbotstorage.table.core.windows.net/;FileEndpoint=https://hiveittenderbotstorage.file.core.windows.net/;";
        //test cs
        
        // var cs = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);
        


        // TableClient client = new TableClient(cs, "Tenders");
        TableClient client = new TableClient(testcs, "tenderstest"); //test


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
        string testcs = "AccountName=hiveittenderbotstorage;AccountKey=MhwmuUUF17wEq7noDPOKz06SspZL5IQ8KG0KXzldMnavXPjZLVbOQGAwQbN8FgoWeacwjbOTcHKu+ddQ34YxGw==;BlobEndpoint=https://hiveittenderbotstorage.blob.core.windows.net/;QueueEndpoint=https://hiveittenderbotstorage.queue.core.windows.net/;TableEndpoint=https://hiveittenderbotstorage.table.core.windows.net/;FileEndpoint=https://hiveittenderbotstorage.file.core.windows.net/;";
        //test cs
        
        // var cs = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);

        // TableClient client = new TableClient(cs, "Tenders");

        TableClient client = new TableClient(testcs, "tenderstest"); //test

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