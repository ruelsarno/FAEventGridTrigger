// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FAEventGridTrigger
{
    public static class Subscription01
    {
        [FunctionName("Subscription01")]
        public async static Task<IActionResult> Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());

            await Controller.AddItemsToContainerAsync(eventGridEvent.Id, eventGridEvent.Subject, eventGridEvent.Topic);
            

            string responseMessage = string.IsNullOrEmpty(eventGridEvent.Subject)
                ? "This HTTP triggered function executed successfully."
                : $"The, {eventGridEvent.Subject}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);

        }
    }
}
