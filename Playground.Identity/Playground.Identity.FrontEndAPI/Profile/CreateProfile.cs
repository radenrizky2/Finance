using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Playground.Identity.DAL;
using Playground.Identity.FrontEndAPI.User;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Playground.Identity.FrontEndAPI.Profile
{
    public class CreateProfile
    {
        private readonly ILogger<CreateProfile> _logger;
        private readonly IUnitOfWork _uow;

        public CreateProfile(ILogger<CreateProfile> log, IUnitOfWork uow)
        {
            _uow ??= uow;
            _logger = log;
        }

    

        [FunctionName("CreateProfile")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
           
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var dataProfile = JsonConvert.DeserializeObject<DAL.Model.Profile>(requestBody);

            var result = await _uow.ProfileRepository.CreateAsync(dataProfile);

            return new OkObjectResult(result);
        }
    }
}

