using System.IO;
using System.Threading.Tasks;
using Demo.Functions.Interfaces;
using Demo.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Demo.Functions.Tests
{
    public class Tests
    {
        private ILogger logger;
        private Mock<IStorageWorker> worker;
        private Mock<IDOOperations> doOperations;
        
        [SetUp]
        public void Setup()
        {
            logger = NullLoggerFactory.Instance.CreateLogger("Tests");
            worker = new Mock<IStorageWorker>();
            doOperations = new Mock<IDOOperations>();
        }
        
        [Test(Description = "Test, if you didn't provide any information for DEV build URL to be able to call it")]
        public async Task NoUrlProvidedAndResultsInNoDataRetrieved()
        {
            var mockRequest = CreateMockRequest("data");
            
            var prepareStatusReport = new PrepareStatusReport(doOperations.Object, 
                worker.Object);
            var result = await prepareStatusReport.RunAsync(mockRequest.Object, 
                new TestableAsyncCollector<SignalRMessage>(), 
                new TestableAsyncCollector<CloudQueueMessage>(), logger);
            
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test(Description = "Test, if logger has an object representation")]
        public void TestIfLoggerNotNull()
        {
            Assert.True(logger != null);
        }
        
        private static Mock<HttpRequest> CreateMockRequest(object body)
        {            
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
 
            var json = JsonConvert.SerializeObject(body);
 
            sw.Write(json);
            sw.Flush();
 
            ms.Position = 0;
 
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);
 
            return mockRequest;
        }
    }
}