using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace Demo.IPRanges
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //download file - https://download.microsoft.com/download/7/1/D/71D86715-5596-4529-9B13-DA13A5DE5B63/ServiceTags_Public_20201207.json
            //confirmation https://www.microsoft.com/en-us/download/confirmation.aspx?id=56519
            HorizontalRule("Download file for Azure IP ranges and Service Tags");
            var fileData = await DownloadFileAsync();

            if (string.IsNullOrEmpty(fileData))
            {
                AnsiConsole.WriteLine("File data is empty, check download url and check it out in browser");
                return;
            }

            var notAllRegions = new List<string>
            {
                "centralus",
                "eastus",
                "eastus2",
                "northcentralus",
                "southcentralus",
                "westcentralus",
                "westus",
                "westus2",
                "westeurope",
                "northeurope"
            };

            // Load the weekly file
            var weeklyFile = JObject.Parse(fileData);
            var values = (JArray) weeklyFile["values"];
            if (values == null)
            {
                AnsiConsole.WriteLine("Values from IP ranges were empty, check file.");
            }
            else
            {
                AnsiConsole.WriteLine($"Found {values.Count} items, doing tranformation to get IP ranges.");

                HorizontalRule("Read IP ranges in all regions");

                foreach (var region in notAllRegions)
                {
                    var azureCloudRegion = $"AzureCloud.{region}";
                    AnsiConsole.WriteLine(azureCloudRegion);

                    var ipList = from v in values
                        where (string) v["name"] == azureCloudRegion
                        select v["properties"]?["addressPrefixes"];

                    foreach (var ip in ipList.Children())
                    {
                        AnsiConsole.WriteLine((string) ip ?? "No IP defined");
                    }
                }
                
                HorizontalRule("Read IP ranges in West Europe region");

                var ipForWE = from region in values
                    where (string) region["name"] == "AzureCloud.westeurope"
                    select region["properties"]?["addressPrefixes"];
                foreach (var ip in ipForWE.Children())
                {
                    AnsiConsole.WriteLine((string) ip ?? "No IP defined");
                }
            }
        }

        private static async Task<string> DownloadFileAsync()
        {
            var urlFileToDownload = Environment.GetEnvironmentVariable("FileUrl");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                using var response = await client.GetAsync(urlFileToDownload);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
            }

            return string.Empty;
        }

        private static void HorizontalRule(string title)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Render(new Rule($"[white bold]{title}[/]").RuleStyle("grey").LeftAligned());
            AnsiConsole.WriteLine();
        }
    }
}