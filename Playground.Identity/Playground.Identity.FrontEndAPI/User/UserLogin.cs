using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Playground.Identity.BLL.UserManagement;
using Playground.Identity.DAL;
using Playground.Identity.FrontEndAPI.User.DTO;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Playground.Identity.FrontEndAPI.User
{
    public class UserLogin
    {
        private readonly ILogger<UserLogin> _logger;
        private readonly IUnitOfWork _uow;
        private readonly UserManager _userManager;

        public UserLogin(ILogger<UserLogin> log, IUnitOfWork uow)
        {
            _uow ??= uow;
            _logger = log;
            _userManager ??= new UserManager(_uow);
        }

        [FunctionName("UserLogin")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserLoginDTO), Description = "DTO for User Login")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<UserLoginDTO>(requestBody);

            // ToDo : Password belum di lakukan proses auth
            var user = await _userManager.GetUserByEmail(data.Email, _logger);
            //var user = (await _uow.UserRepository.GetAsync(
            //   predicate: p => p.Email == data.Email)).Items.FirstOrDefault();

            // ToDo : Claims Lainnya belum di sertakan jika ada
            var result = _userManager.GenerateJWTToken(user.Id, user.FullName, null);

            return new OkObjectResult(result);
        }
    }
}

