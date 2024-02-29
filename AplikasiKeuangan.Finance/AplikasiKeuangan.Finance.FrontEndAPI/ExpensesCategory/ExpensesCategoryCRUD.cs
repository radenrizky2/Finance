using AplikasiKeuangan.Finance.BLL.ExpensesCategoryManagement;
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

namespace AplikasiKeuangan.Finance.FrontEndAPI.ExpensesCategory
{
    public class ExpensesCategoryCRUD
    {
        private readonly ILogger<ExpensesCategoryCRUD> _logger;
        private readonly IUnitOfWork _uow;
        private readonly ExpensesCategroyManager _expensesCategoryManager;

        public ExpensesCategoryCRUD(ILogger<ExpensesCategoryCRUD> log, IUnitOfWork uow)
        {
            _uow ??= uow;
            _expensesCategoryManager ??= new ExpensesCategroyManager(_uow);
            _logger = (ILogger<ExpensesCategoryCRUD>)log;
        }
        [FunctionName("CreateExpensesCategory")]
        [OpenApiOperation(operationId: "CreateExpensesCategory", tags: new[] { "Expenses Category CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ExpensesCategoryDTO), Description = "Add Category")]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateExpensesCategory(
       [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Category/Create")] HttpRequest req,
       [AccessToken] ClaimsPrincipal principal)
        {
            _logger.LogInformation("Creating a new category.");

            // Extract userId from the token
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation($"userId: {userId}");
            if (string.IsNullOrEmpty(userId))
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var expensesCategory = JsonConvert.DeserializeObject<DAL.Model.ExpensesCategory>(requestBody);

            // Assign the userId to the category
            expensesCategory.UserId = userId;

            var createdExpensesCategory = await _expensesCategoryManager.CreateExpensesCategory(expensesCategory, _logger, userId);

            return new OkObjectResult(createdExpensesCategory);
        }


        [FunctionName("GetExpensesCategoryById")]
        [OpenApiOperation(operationId: "GetExpensesCategoryById", tags: new[] { "ExpensesCategory CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiParameter(name: "categoryId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The CategoryID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DAL.Model.ExpensesCategory), Description = "The Category object")]
        public async Task<IActionResult> GetExpensesCategoryById(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ExpensesCategory/{categoryId}")] HttpRequest req,
           string categoryId,
           [AccessToken] ClaimsPrincipal principal
         )
        {
            _logger.LogInformation($"Getting expenses category with ID: {categoryId}");

            var result = await _expensesCategoryManager.GetExpensesCategoryById(categoryId, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("GetExpensesCategory")]
        [OpenApiOperation(operationId: "GetExpensesCategory", tags: new[] { "Expenses Category CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiParameter(name: "CT", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Continuation Token")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<ExpensesCategoryDTO>), Description = "The OK response")]
        public async Task<IActionResult> GetExpensesCategory(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Category")] HttpRequest req,
           [AccessToken] ClaimsPrincipal principal
           )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string continuationToken = req.Query["CT"];

            var result = await _expensesCategoryManager.GetAllCategory(continuationToken, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("UpdateExpensesCategory")]
        [OpenApiOperation(operationId: "UpdateExpensesCategory", tags: new[] { "Expenses Category CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ExpensesCategoryDTO), Description = "Expenses Category DTO Update")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> UpdateExpensesCategory(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Category/Update/{categoryId}")] HttpRequest req,
        string categoryId,
        [AccessToken] ClaimsPrincipal principal
        )
        {
            _logger.LogInformation($"Updating expenses category with ID: {categoryId}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedCategory = JsonConvert.DeserializeObject<DAL.Model.ExpensesCategory>(requestBody);

            var result = await _expensesCategoryManager.UpdateExpensesCategory(categoryId, updatedCategory, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("DeleteExpensesCategory")]
        [OpenApiOperation(operationId: "DeleteExpensesCategory", tags: new[] { "Expenses Category CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ExpensesDTO), Description = "Expenses Category Delete")]
        [OpenApiParameter(name: "categoryId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The CategoryID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteExpensesCategory(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "ExpensesCategory/Delete/{categoryId}")] HttpRequest req,
        string categoryId,
        [AccessToken] ClaimsPrincipal principal
        )
        {
            _logger.LogInformation($"Deleting category with ID: {categoryId}");

            await _expensesCategoryManager.DeleteCategory(categoryId, _logger);

            return new OkResult();
        }
    }
}
