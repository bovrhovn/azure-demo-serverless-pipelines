using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Interfaces;
using Demo.Web.Models;
using Demo.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demo.Web.Pages.Report
{
    public class DownloadPageModel : PageModel
    {
        private readonly IStorageWorker worker;
        private readonly ILogger<DownloadPageModel> logger;
        private readonly string reportsContainer;

        public DownloadPageModel(IStorageWorker worker,
            IOptions<StorageOptions> storageOptions,
            ILogger<DownloadPageModel> logger)
        {
            this.worker = worker;
            reportsContainer = storageOptions.Value.ReportsContainer;
            this.logger = logger;
        }

        public async Task OnGetAsync()
        {
            logger.LogInformation($"Loading download page and getting back reports from {reportsContainer}");
            var reports = await worker.GetReportsAsync(reportsContainer);
            foreach (var currentReport in reports)
            {
                Reports.Add(new ReportViewModel
                {
                    Name = currentReport.Name,
                    Url = currentReport.Uri
                });
            }
            logger.LogInformation($"Load {Reports.Count} items");
        }

        [BindProperty]
        public List<ReportViewModel> Reports { get; } = new List<ReportViewModel>();
    }
}