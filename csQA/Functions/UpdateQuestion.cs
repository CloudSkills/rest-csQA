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
    public class UpdateQuestion
    {

        private readonly ILogger<UpdateQuestion> _logger;
        private readonly Constants _settings;
        private CosmosClient _cosmosClient;
        private Container _container;

        public UpdateQuestion(ILogger<UpdateQuestion> logger, IOptions<Constants> options, CosmosClient cosmosClient)
        {
            _logger = logger;
            _settings = options.Value;
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_settings.CosmosDbDatabaseName,_settings.CosmosDbContainerName);

        }

        [FunctionName("UpdateQuestion")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "questions/{questionId}")] HttpRequest req, string questionId)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<Question>(requestBody);
            
            ItemResponse<Question> item = await _container.UpsertItemAsync(input);

            return new OkObjectResult(item.Resource);

        }
    }
}
