using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Nexus.Base.Identity;

namespace Playground.Identity.FrontEndAPI.User
{
    public class UserLogin
    {
        private readonly ILogger<UserLogin> _logger;

        public UserLogin(ILogger<UserLogin> log)
        {
            _logger = log;
        }

        [FunctionName("UserLogin")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return await Login(req);
            //return new OkObjectResult(TokenManager.GenerateKeyPair());
        }

        private static async Task<IActionResult> Login(HttpRequest req)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var tokenClaims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, "thisIsUserId"),
                            new Claim(ClaimTypes.Name, "thisIsUserName")
                        };

            var tokenResult = TokenManager.GenerateToken(tokenClaims);

            return new OkObjectResult(tokenResult);
        }
    }
}

