using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using csQA.DTO;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Queue;

namespace csQA.Functions
{
    public class GetQuestionById
    {

        private readonly ILogger<GetQuestionById> _logger;
        private readonly Constants _settings;
        private CosmosClient _cosmosClient;
        private Container _container;

        public GetQuestionById(ILogger<GetQuestionById> logger, IOptions<Constants> options, CosmosClient cosmosClient)
        {
            _logger = logger;
            _settings = options.Value;
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_settings.CosmosDbDatabaseName,_settings.CosmosDbContainerName);

        }

        [FunctionName("GetQuestionById")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", 
                Route = "questions/{questionId}")] HttpRequest req, 
                string questionId)
        {
            Question question = _container.GetItemLinqQueryable<Question>(true)
                .Where(q => q.Id == questionId)
                .AsEnumerable().FirstOrDefault();
            
            return new OkObjectResult(question);

        }
    }
}
