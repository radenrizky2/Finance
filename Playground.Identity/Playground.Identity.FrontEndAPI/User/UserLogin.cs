using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Nexus.Base.Identity;
using Playground.Identity.FrontEndAPI.User.DTO;
using StackExchange.Redis;
using static Playground.Identity.DAL.Repositories;

namespace Playground.Identity.FrontEndAPI.User
{
    public class UserLogin
    {
        private readonly ILogger<UserLogin> _logger;
        private static CosmosClient _client;

        public UserLogin(ILogger<UserLogin> log, CosmosClient client)
        {
            _client ??= client;
            _logger = log;
        }

        [FunctionName("UserLogin")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserLoginDTO), Description = "DTO for User Login")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<UserLoginDTO>(requestBody);

            // ToDo : Password belum di lakukan sesuatu
            var repsUser = new UserRepository(_client);

            var user = (await repsUser.GetAsync(
                predicate: p => p.Email == data.Email))
                .Items.ToList().FirstOrDefault();

            // ToDo : Claims Lainnya belum di sertakan jika ada
            var result = BLL.UserManagement.UserManager.GenerateJWTToken(user.Id, user.FullName, null);

            return new OkObjectResult(result);
        }
    }
}

