using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Demo.Functions.Interfaces;
using Demo.Functions.Models;
using Newtonsoft.Json;

namespace Demo.Functions.Services
{
    public class DOOperationService : IDOOperations
    {
        private readonly string pat;

        public DOOperationService(string pat)
        {
            this.pat = pat;
        }
        
        public async Task<DOBuild> GetLatestBuildAsync()
        {
            var response =
                await GetDataAsync(Environment.GetEnvironmentVariable("ProjectUrl"));
            try
            {
                var buildModel = JsonConvert.DeserializeObject<BuildModel>(response);

                if (buildModel.List == null || buildModel.List.Count == 0) return null;
                
                var build = buildModel.List.OrderByDescending(d => d.FinishTime)
                    .FirstOrDefault();
                
                if (build == null) return null;
                
                return new DOBuild
                {
                    Name = build.Queue?.Name ?? "No public data",
                    Status = build.Status,
                    FinishTime = build.FinishTime,
                    StartTime = build.StartTime,
                    IsHosted = build.Queue?.Pool?.IsHosted ?? true
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }
        
        public async Task<string> GetDataAsync(string url)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}",
                    "", pat))));

            using var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}