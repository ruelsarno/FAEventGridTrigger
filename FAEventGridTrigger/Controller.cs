using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.Cosmos;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace FAEventGridTrigger
{
    public class Controller
    {
        private static readonly string ConnString = Environment.GetEnvironmentVariable("ConnectionString");

        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("URI");

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");

        // The Cosmos client instance
        private static CosmosClient cosmosClient;

        // The database we will create
        private static Database database;

        // The container we will create.
        private static Container container;

        // The name of the database and container we will create
        private static string databaseId = "cosmosdb-test";
        private static string containerId = "events";

        public static async Task AddItemsToContainerAsync(String Id, String Subject, String Topic)
        {
            CosmosClient cosmosClient = new CosmosClient(ConnString);
            database = cosmosClient.GetDatabase(databaseId);
            container = database.GetContainer(containerId);

            Events topic = new Events();
            topic.Id = Id;
            topic.Subject = Subject;
            topic.Topic = Topic;

            try
            {

                // Read the item to see if it exists.                  
                ItemResponse<Events> basicResponse = await container.ReadItemAsync<Events>(topic.Id, new PartitionKey(topic.Subject));
                Console.WriteLine("Item in database with id: {0} already exists\n", basicResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine(ex.ToString());
                //Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                await container.CreateItemAsync<Events>(topic, new PartitionKey(topic.Subject));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n");
            }           
        }

    }
}
