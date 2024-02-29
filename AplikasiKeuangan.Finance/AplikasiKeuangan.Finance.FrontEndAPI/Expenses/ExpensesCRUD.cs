using AplikasiKeuangan.Finance.BLL.ExpensesCategoryManagement;
using AplikasiKeuangan.Finance.BLL.ExpensesManagement;
using AplikasiKeuangan.Finance.DAL;
using AplikasiKeuangan.Finance.FrontEndAPI.Expenses.DTO;
using AplikasiKeuangan.Finance.FrontEndAPI.ExpensesCategory.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Nexus.Base.Identity;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AplikasiKeuangan.Finance.FrontEndAPI.Expenses
{
    public class ExpensesCRUD
    {
        private readonly ILogger<ExpensesCRUD> _logger;
        private readonly IUnitOfWork _uow;
        private readonly ExpensesManager _expensesManager;

        public ExpensesCRUD(ILogger<ExpensesCRUD> log, IUnitOfWork uow)
        {
            _uow ??= uow;
            _expensesManager ??= new ExpensesManager(_uow);
            _logger = log;
        }
        [FunctionName("CreateExpenses")]
        [OpenApiOperation(operationId: "CreateExpenses", tags: new[] { "Expenses CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ExpensesDTO), Description = "Add Expenses")]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateExpenses(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Expenses/Create")] HttpRequest req,
        [AccessToken] ClaimsPrincipal principal)
        {
            _logger.LogInformation("Creating a new expenses.");

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation($"userId: {userId}");
            if (string.IsNullOrEmpty(userId))
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation("req body {requestBody}");
            if (string.IsNullOrEmpty(requestBody))
            {
                return new BadRequestObjectResult("Request body is empty.");
            }
            DAL.Model.Expenses expenses = null;
            expenses = JsonConvert.DeserializeObject<DAL.Model.Expenses>(requestBody);
            _logger.LogInformation("Category id {expenses}");
            var categoryExists = await _expensesManager.CategoryExists(expenses.CategoryId, _logger);
            if (!categoryExists)
            {
                return new BadRequestObjectResult("Category with the provided ID does not exist.");
            }

            // Assign the userId to the category
            expenses.UserId = userId;

            var createdExpenses = await _expensesManager.CreateExpenses(expenses, _logger, userId);

            return new OkObjectResult(createdExpenses);
        }

        [FunctionName("GetExpenses")]
        [OpenApiOperation(operationId: "GetExpenses", tags: new[] { "Expenses CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiParameter(name: "CT", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Continuation Token")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<ExpensesDTO>), Description = "The OK response")]
        public async Task<IActionResult> GetExpenses(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Expenses")] HttpRequest req,
           [AccessToken] ClaimsPrincipal principal
           )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string continuationToken = req.Query["CT"];

            var result = await _expensesManager.GetAllExpenses(continuationToken, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("GetExpensesById")]
        [OpenApiOperation(operationId: "GetExpensesById", tags: new[] { "Expenses CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiParameter(name: "expensesId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The ExpensesID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DAL.Model.Expenses), Description = "The Expenses object")]
        public async Task<IActionResult> GetExpensesById(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Expenses/{expensesId}")] HttpRequest req,
           string expensesId,
           [AccessToken] ClaimsPrincipal principal
         )
        {
            _logger.LogInformation($"Getting expenses with ID: {expensesId}");

            var result = await _expensesManager.GetExpensesById(expensesId, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("UpdateExpenses")]
        [OpenApiOperation(operationId: "UpdateExpenses", tags: new[] { "Expenses CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ExpensesDTO), Description = "Expenses Update")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> UpdateExpenses(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Expenses/Update/{expensesId}")] HttpRequest req,
        string expensesId,
        [AccessToken] ClaimsPrincipal principal
        )
        {
            _logger.LogInformation($"Updating expenses with ID: {expensesId}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation("req body {requestBody}");
            if (string.IsNullOrEmpty(requestBody))
            {
                return new BadRequestObjectResult("Request body is empty.");
            }
            DAL.Model.Expenses updatedExpenses = null;
            updatedExpenses = JsonConvert.DeserializeObject<DAL.Model.Expenses>(requestBody);
            _logger.LogInformation("Category id {expenses}");
            var categoryExists = await _expensesManager.CategoryExists(updatedExpenses.CategoryId, _logger);
            if (!categoryExists)
            {
                return new BadRequestObjectResult("Category with the provided ID does not exist.");
            }
            var result = await _expensesManager.UpdateExpenses(expensesId, updatedExpenses, _logger);

            return new OkObjectResult(result);
        }
        [FunctionName("DeleteExpenses")]
        [OpenApiOperation(operationId: "DeleteExpenses", tags: new[] { "Expenses CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ExpensesDTO), Description = "Expenses Delete")]
        [OpenApiParameter(name: "expensesId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The ExpensesID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteExpenses(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Expenses/Delete/{expensesId}")] HttpRequest req,
        string expensesId,
        [AccessToken] ClaimsPrincipal principal
        )
        {
            _logger.LogInformation($"Deleting category with ID: {expensesId}");

            await _expensesManager.DeleteExpenses(expensesId, _logger);

            return new OkResult();
        }
    }
}
