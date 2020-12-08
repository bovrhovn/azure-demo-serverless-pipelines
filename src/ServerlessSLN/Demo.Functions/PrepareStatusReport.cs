using System;
using System.IO;
using System.Threading.Tasks;
using Demo.Functions.Interfaces;
using Demo.Functions.Models;
using Demo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Demo.Functions
{
    public class PrepareStatusReport
    {
        private readonly IDOOperations operations;
        private readonly IStorageWorker worker;

        public PrepareStatusReport(IDOOperations operations, IStorageWorker worker)
        {
            this.operations = operations;
            this.worker = worker;
        }

        [FunctionName("PrepareStatusReport")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req,
            [SignalR(HubName = "messages", ConnectionStringSetting = "AzureSignalRConnectionString")]
            IAsyncCollector<SignalRMessage> signalRMessages,
            [Queue("lambada-emails")] IAsyncCollector<CloudQueueMessage> messages, ILogger log)
        {
            log.LogInformation("Starting to prepare report");
            log.LogInformation("Getting latest build");

            try
            {
                var latestBuild = await operations.GetLatestBuildAsync();

                if (latestBuild == null) return new NoContentResult();

                //prepare report in html
                log.LogInformation("Getting html for preparing the result");
                var html = await worker.DownloadAsStringAsync("reports.html");

                //fill the data
                var replaced = html.Replace("##BUILDNAME", latestBuild.Name);
                string table = $"<tr><td>Status: {latestBuild.Status}</td>" +
                               $"<td>Is hosted build engine: {(latestBuild.IsHosted ? "YES" : "NO")}</td>" +
                               $"<td>Start time: {latestBuild.StartTime}</td>" +
                               $"<td>Finish time: {latestBuild.FinishTime}</td></tr>";

                replaced = replaced.Replace("##INFO", table);
                replaced = replaced.Replace("##FOOTER", $"Generated from Azure Function at {DateTime.Now}");

                //save it to storage account and table
                log.LogInformation(
                    "Switching between containers to serve data from Reports container and to save data");
                worker.Container = "reports";

                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(replaced);
                await writer.FlushAsync();
                stream.Position = 0;

                var latestBuildName = $"{latestBuild.Name}-{Guid.NewGuid()}.html"
                    .Replace(" ", "");
                await worker.UploadFileAsync(latestBuildName, stream);

                var fileUrl = await worker.GetFileUrl(latestBuildName, false);

                //send email
                var defaultEmailFrom = Environment.GetEnvironmentVariable("DefaultEmailFrom");
                log.LogInformation($"Sending email to me {defaultEmailFrom}");
                var content = $"Report has been generated for {latestBuild.Name}. Check it out at {fileUrl}";
                var emailModel = new EmailModel
                {
                    From = defaultEmailFrom,
                    To = Environment.GetEnvironmentVariable("DefaultEmailTo"),
                    Content = content,
                    Subject = $"Report generated {latestBuild.Name}"
                };

                var emailMessage = JsonConvert.SerializeObject(emailModel);
                await messages.AddAsync(new CloudQueueMessage(emailMessage));
                log.LogInformation($"Email was sent from {defaultEmailFrom}");

                //send to signalr for realtime updates
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "messages",
                        Arguments = new object[] {content}
                    });

                log.LogInformation($"Information has been sent to signalr at {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }

            return new OkObjectResult("Report has been generated and saved");
        }
    }
}