using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Playground.Identity.DAL;

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

            var resultDB = await _uow.ProfileRepository.GetAsync(
                selector: s => new DAL.Model.Profile()
                {
                    FullName = s.FullName,
                    City = s.City,
                    Edisi= s.Edisi,
                    Id = s.Id,
                    ActiveFlag = s.ActiveFlag,
                },
                orderBy: ob => ob.OrderByDescending(o => o.Edisi),
                predicate: p => p.FullName == requestDTO.FullName,
                partitionKeys: new Dictionary<string, string>() { { "city", requestDTO.City } },
                continuationToken: requestDTO.ContinuationToken,
                //enableCrossPartition: true
                //cachePrefix: $"Profile-{city}-{name}",
                //cacheExpiry: new TimeSpan(00, 05, 00)
                isDebug: true,
                //pageNumber: 2
                usePaging: true,
                pageSize: 2
                );

            var continuationToken = resultDB.ContinuationToken;
            var result = resultDB.Items.ToList();

            return new OkObjectResult(resultDB);
        }
    }
}

