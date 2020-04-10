 using System;
using System.IO;
using System.Threading.Tasks;
 using csQA.DTO;
 using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
 using Microsoft.Azure.Cosmos;
 using Microsoft.Extensions.Configuration;
 using Microsoft.Extensions.Logging;
 using Microsoft.Extensions.Options;
 using Newtonsoft.Json;

namespace csQA.Functions
{
    public class CreateQuestion
    {
        private readonly ILogger _logger;
        private readonly Constants _settings;
        private CosmosClient _cosmosClient;

        private Database _database;
        private Container _container;

        public CreateQuestion(
            ILogger<CreateQuestion> logger,
            IOptions<Constants> options,
            CosmosClient cosmosClient)
        {
            _logger = logger;
            _settings = options.Value;
            _cosmosClient = cosmosClient;

            _database = _cosmosClient.GetDatabase(_settings.CosmosDbDatabaseName);
            _container = _database.GetContainer(_settings.CosmosDbContainerName);
        }

        [FunctionName("CreateQuestion")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "questions")] HttpRequest req)
        {
            IActionResult returnValue = null;

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var input = JsonConvert.DeserializeObject<Question>(requestBody);

                var question = new Question
                {
                    Id = Guid.NewGuid().ToString(),
                    PartitionKey = "QUESTIONS",
                    Title = input.Title,
                    Answer = input.Answer,
                    Links = input.Links,
                    Attachments = input.Attachments
                };

                ItemResponse<Question> item =
                    await _container.CreateItemAsync(question, new PartitionKey("QUESTIONS"));
              
                _logger.LogInformation("Item inserted");
                _logger.LogInformation($"This query cost: {item.RequestCharge} RU/s");
                
                returnValue = new OkObjectResult(question);

            }
            catch (Exception e)
            {
                _logger.LogError($"Could not insert item. Exception thrown: {e.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return returnValue;

        }
    }
}
