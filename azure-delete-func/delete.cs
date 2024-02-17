using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Microsoft.Azure.Storage;
using Microsoft.Azure;
using Microsoft.Azure.Storage.Blob;
using System.Text.Json;
using System.Threading.Tasks;

namespace azure_func_delete
{
    public class delete
    {
        [FunctionName("delete")]
        public void Run([QueueTrigger("pics-to-delete", Connection = "AzureStorageConnection")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            log.LogInformation(Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING"));
            JsonDocument json = JsonDocument.Parse(myQueueItem);
            Task task = json.Deserialize<Task>();
            log.LogInformation(task.heroName);

            var blobClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING"));
            BlobContainerClient heroesContainer = blobClient.GetBlobContainerClient("heroes");
            BlobContainerClient alteregoContainer = blobClient.GetBlobContainerClient("alteregos");

            var heroImageJPEG = $"{task.heroName.Replace(" ", "-").ToLower()}.jpeg";
            var heroImageJPG = $"{task.heroName.Replace(" ", "-").ToLower()}.jpg";
            var alteregoImage = $"{task.alterEgoName.Replace(" ", "-").ToLower()}.png";
            Console.WriteLine($"Looking for {heroImageJPEG} or {heroImageJPG}");
            var heroBlobJPEG = heroesContainer.GetBlobClient(heroImageJPEG);
            var heroBlobJPG = heroesContainer.GetBlobClient(heroImageJPG);
            var alteregoBlob = alteregoContainer.GetBlobClient(alteregoImage);

            if (heroBlobJPEG.Exists() || heroBlobJPG.Exists() || alteregoBlob.Exists())
            {
                Console.WriteLine($"Found it! {task.heroName}");

                //Delete the old blob
                heroBlobJPEG.DeleteIfExists();
                heroBlobJPG.DeleteIfExists();
                alteregoBlob.DeleteIfExists();

                Console.WriteLine($"Thanos slap the fingers and");

                Console.WriteLine($"The universe is a better place now");
              
            }
            else
            {
                Console.WriteLine($"No body here");
                Console.WriteLine($"Dismiss task.");
                //Delete message from the queue
               
            }

        }
      
    //var heroBlobJPEG = heroesContainer.GetBlobClient(name);
    //var heroBlobJPG = heroesContainer.GetBlobClient(name);
    //var alteregoBlob = alteregoContainer.GetBlobClient(name);

    //heroBlobJPEG.DeleteIfExists();
    //heroBlobJPG.DeleteIfExists();
    //alteregoBlob.DeleteIfExists();
    //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AZURE_CONECTION"));
    //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
    //CloudBlobContainer container = blobClient.GetContainerReference("heroes");
    //BlobContainerClient heroesContainer = blobClient.GetBlobContainerClient("heroes");
    //BlobContainerClient alteregoContainer = blobClient.GetBlobContainerClient("alteregos");
    //var heroBlobJPEG = heroesContainer.GetBlobClient("");
    //var heroBlobJPG = heroesContainer.GetBlobClient("");
    //var alteregoBlob = alteregoContainer.GetBlobClient("");

}
        class Task
        {
            public String heroName { get; set; }
            public String alterEgoName { get; set; }
        }
    }


