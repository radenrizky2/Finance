using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using Playground.Identity.BLL.ProfileManagement;
using Playground.Identity.DAL;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Playground.Identity.FrontEndAPI.Profile
{
    public class GetProfile
    {
        private readonly ILogger<GetProfile> _logger;
        private readonly IUnitOfWork _uow;

        public GetProfile(ILogger<GetProfile> log, IUnitOfWork uow)
        {
            _uow ??= uow;
            _logger = log;
        }

        [FunctionName("GetProfile")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "profileLists")] HttpRequest req
           )
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestDTO = JsonConvert.DeserializeObject<RequestDTO>(requestBody);

            if (requestDTO == null)
            {
                return new BadRequestObjectResult("Data Not Exist");
            }

            var ProfileSVC = new ProfileManager(_uow);

            PageResult<DAL.Model.Profile> resultDB = await ProfileSVC.GetProfileData(requestDTO);

            return new OkObjectResult(resultDB);
        }

       
    }
}

