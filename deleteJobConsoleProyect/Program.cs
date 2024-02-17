using System;
using System.Text.Json;
using System.Threading;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;


Console.WriteLine("Hello to the QueueProcessor!");

var queueClient = new QueueClient(Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING"), "pics-to-delete");

queueClient.CreateIfNotExists();

while (true)
{
    QueueMessage message = queueClient.ReceiveMessage();

    if (message != null)
    {
        Console.WriteLine($"Message received {message.Body}");

        var task = JsonSerializer.Deserialize<Task>(message.Body);

        Console.WriteLine($"Oh no thanos is looking for a hero!!!!");

        if (task.heroName != null)
        {
            //Create a Blob service client
            var blobClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING"));

            //Get container client
            BlobContainerClient heroesContainer = blobClient.GetBlobContainerClient("heroes");
            BlobContainerClient alteregoContainer = blobClient.GetBlobContainerClient("alteregos");

            //Get blob with old name
            var heroImageJPEG = $"{task.heroName.Replace(" ","-").ToLower()}.jpeg";
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

                Console.WriteLine($"The universe is a better place now");
                //Delete message from the queue
                queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
            }
            else
            {
                Console.WriteLine($"No body here");
                Console.WriteLine($"Dismiss task.");
                //Delete message from the queue
                queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
            }

        }
        else
        {
            Console.WriteLine($"Bad message. Delete it");
            //Delete message from the queue
            queueClient.DeleteMessage(message.MessageId, message.PopReceipt);

        }
    }
    else
    {
        Console.WriteLine($"Let's wait 5 seconds");
        Thread.Sleep(5000);
    }

}

class Task
{
    public string heroName { get; set; }
    public string alterEgoName { get; set; }
}