using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Demo.Cli.Models;
using Newtonsoft.Json;
using Spectre.Console;

namespace Demo.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HorizontalRule("Getting info from REST calls");
            await GetProjects();
            HorizontalRule("List build in specific project");
            await ListBuildsAsync();
        }

        public static async Task ListBuildsAsync()
        {
            var response =
                await GetDataAsync(
                    "https://dev.azure.com/cee-csa-projects/Azure Demos/_apis/build/builds?api-version=6.1-preview.6");
            try
            {
                var buildModel = JsonConvert.DeserializeObject<BuildModel>(response);
                var buildRepresentation = new Table()
                    .Border(TableBorder.Square)
                    .BorderColor(Color.Blue)
                    .AddColumn(new TableColumn("Status"))
                    .AddColumn(new TableColumn("Queue info"))
                    .AddColumn(new TableColumn("Start time"))
                    .AddColumn(new TableColumn("End time"))
                    .AddColumn(new TableColumn("Queue time"));
                foreach (var build in buildModel.List.OrderByDescending(d => d.FinishTime))
                {
                    buildRepresentation.AddRow(build.Status, build.Queue?.Name ?? "No data",
                        build.StartTime.ToString(CultureInfo.InvariantCulture),
                        build.FinishTime.ToString(CultureInfo.InvariantCulture),
                        build.QueueTime.ToString(CultureInfo.InvariantCulture));
                }

                AnsiConsole.Render(buildRepresentation);
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
            }
        }

        public static async Task GetProjects()
        {
            try
            {
                var responseBody = await GetDataAsync("https://dev.azure.com/cee-csa-projects/_apis/projects");

                var projects = JsonConvert.DeserializeObject<Projects>(responseBody);

                var projectRepresentation = new Table()
                    .Border(TableBorder.Square)
                    .BorderColor(Color.Red)
                    .AddColumn(new TableColumn("Name"))
                    .AddColumn(new TableColumn("Description"))
                    .AddColumn(new TableColumn("Last updated"));

                foreach (var project in projects.List)
                {
                    projectRepresentation.AddRow(project.Name, project.Description ?? "",
                        project.LastUpdateTime.ToString(CultureInfo.InvariantCulture));
                }

                AnsiConsole.Render(projectRepresentation);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        public static async Task<string> GetDataAsync(string url)
        {
            var personalaccesstoken = Environment.GetEnvironmentVariable("Pat");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}",
                    "", personalaccesstoken))));

            using var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private static void HorizontalRule(string title)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Render(new Rule($"[white bold]{title}[/]").RuleStyle("grey").LeftAligned());
            AnsiConsole.WriteLine();
        }
    }
}