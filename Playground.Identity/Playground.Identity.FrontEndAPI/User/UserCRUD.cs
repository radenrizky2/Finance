using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Playground.Identity.FrontEndAPI.User.DTO;
using static Playground.Identity.DAL.Repositories;

namespace Playground.Identity.FrontEndAPI.User
{
    public class UserCRUD
    {
        private readonly ILogger<UserLogin> _logger;
        private static CosmosClient _client;

        public UserCRUD(ILogger<UserLogin> log, CosmosClient client)
        {
            _client ??= client;
            _logger = log;
        }

        [FunctionName("CreateUser")]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { "User CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserDTO), Description = "DTO for User Login")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "User/Create")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<DAL.Model.User>(requestBody);

            var repsUser = new UserRepository(_client);
            var result = await BLL.UserManagement.UserManager.CreateUser(repsUser, data, _logger, "UserCRUD");

            return new OkObjectResult(result);
        }


        [FunctionName("GetUserById")]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { "User CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The UserId")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(UserDTO), Description = "The OK response")]
        public async Task<IActionResult> GetUserById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/{userId}")] HttpRequest req,
            string userId
            )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var repsUser = new UserRepository(_client);
            var result = await BLL.UserManagement.UserManager.GetUserById(repsUser, userId, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("GetUsers")]
        [OpenApiOperation(operationId: "GetUsers", tags: new[] { "User CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "CT", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Continuation Token")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<UserDTO>), Description = "The OK response")]
        public async Task<IActionResult> GetUsers(
          [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User")] HttpRequest req
          )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string continuationToken = req.Query["CT"];
            var repsUser = new UserRepository(_client);
            var result = await BLL.UserManagement.UserManager.GetAllUser(repsUser, continuationToken, _logger);

            return new OkObjectResult(result);
        }
    }
}

