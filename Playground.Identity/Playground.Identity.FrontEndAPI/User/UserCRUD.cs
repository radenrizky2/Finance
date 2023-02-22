using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Playground.Identity.BLL.UserManagement;
using Playground.Identity.DAL;
using Playground.Identity.FrontEndAPI.User.DTO;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Playground.Identity.FrontEndAPI.User
{
    public class UserCRUD
    {
        private readonly ILogger<UserLogin> _logger;
        private readonly IUnitOfWork _uow;
        private readonly UserManager _userManager;

        public UserCRUD(ILogger<UserLogin> log, IUnitOfWork uow)
        {
            _uow ??= uow;
            _userManager ??= new UserManager(_uow);
            _logger = log;
        }

        [FunctionName("CreateUser")]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { "User CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserDTO), Description = "DTO for User Login")]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "User/Create")] HttpRequest req,
            ClaimsPrincipal principal)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<DAL.Model.User>(requestBody);

            var result = await _userManager.CreateUser(data, _logger, "UserCRUD");

            return new OkObjectResult(result);
        }


        [FunctionName("GetUserById")]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { "User CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The UserId")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(UserDTO), Description = "The OK response")]
        public async Task<IActionResult> GetUserById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/{userId}")] HttpRequest req,
            string userId,
            ClaimsPrincipal principal
            )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _userManager.GetUserById(userId, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("GetUsers")]
        [OpenApiOperation(operationId: "GetUsers", tags: new[] { "User CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiParameter(name: "CT", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Continuation Token")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<UserDTO>), Description = "The OK response")]
        public async Task<IActionResult> GetUsers(
          [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User")] HttpRequest req,
          ClaimsPrincipal principal
          )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string continuationToken = req.Query["CT"];

            var result = await _userManager.GetAllUser(continuationToken, _logger);

            return new OkObjectResult(result);
        }
    }
}

