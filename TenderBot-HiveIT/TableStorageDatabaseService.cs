using Azure;
using Azure.Data.Tables;

namespace TenderBot_HiveIT;

public class TableStorageDatabaseService : IDatabaseService
{
    public bool CheckIfNew(string pageid)
    {


        TableClient client = GetTableClient(); //test


        Pageable<TableEntity> entities = client.Query<TableEntity>(filter: $"ID eq '{pageid}'");

        int counter = 0;

        foreach (TableEntity unused in entities)
        {
                
            counter = counter + 1;
            if (counter > 0)
            {
                return false;
            }
        }

        return true;
    }

    private TableClient GetTableClient()
    {
        // TableClient client = new TableClient(cs, "Tenders");

        TableClient client = new TableClient(GetConnectionStringFromAzure(), "tenderstest"); //test

        return client;

    }
    
    private TableClient GetFavoritesTableClient()
    {
        TableClient client = new TableClient(GetConnectionStringFromAzure(), "Tenders"); //test

        return client;

    }

    private string? GetConnectionStringFromAzure()
    {
        string cs = "AccountName=hiveittenderbotstorage;AccountKey=MhwmuUUF17wEq7noDPOKz06SspZL5IQ8KG0KXzldMnavXPjZLVbOQGAwQbN8FgoWeacwjbOTcHKu+ddQ34YxGw==;BlobEndpoint=https://hiveittenderbotstorage.blob.core.windows.net/;QueueEndpoint=https://hiveittenderbotstorage.queue.core.windows.net/;TableEndpoint=https://hiveittenderbotstorage.table.core.windows.net/;FileEndpoint=https://hiveittenderbotstorage.file.core.windows.net/;";

        // var cs = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);

        return cs;
    }
    
    public bool CheckIfInFavorites(string id)
    {
        
        
        TableClient client = GetFavoritesTableClient();

        Pageable<TableEntity> entities = client.Query<TableEntity>(filter: $"ID eq '{id}'");
        
        int counter = 0;

        foreach (TableEntity unused in entities)
        {
                
            counter = counter + 1;
            if (counter > 0)
            {
                return true;
            }
        }

        return false;
    }

    public void StoreDetails(Details details)
    {
        
        TableClient client = GetTableClient();
        
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
    
    public void StoreFavorites(string id, string username)
    {
        TableClient client = GetFavoritesTableClient();

        var entity = new TableEntity("Tenders", id)
        {
            { "ID", id },
            { "Name", username }
        };
        client.AddEntity(entity);
    }

    public string? GetUrlForMoreInfo(String id)
    {


        TableClient client = GetTableClient(); //test

        List<TableEntity> entities = client.Query<TableEntity>(filter: $"ID eq '{id}'").ToList();
        string? url = "";

        foreach (var entity in entities)
        {
            url = entity["Link"].ToString();
        }
        return url;
    }
    
    public string? GetUsernameFromFavorites(String id)
    {
        
        TableClient client = GetFavoritesTableClient();

        List<TableEntity> entities = client.Query<TableEntity>(filter: $"ID eq '{id}'").ToList();
        string? username = "";

        foreach (var entity in entities)
        {
            username = entity["Name"].ToString();
        }

        return username;
    }
    
}