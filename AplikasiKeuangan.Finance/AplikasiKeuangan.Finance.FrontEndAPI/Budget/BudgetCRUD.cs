using AplikasiKeuangan.Finance.BLL.BudgetManagement;
using AplikasiKeuangan.Finance.DAL;
using AplikasiKeuangan.Finance.FrontEndAPI.Budget.DTO;
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
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AplikasiKeuangan.Finance.FrontEndAPI.Budget
{
    public class BudgetCRUD
    {
        private readonly ILogger<BudgetCRUD> _logger;
        private readonly IUnitOfWork _uow;
        private readonly BudgetManager _budgetManager;

        public BudgetCRUD(ILogger<BudgetCRUD> log, IUnitOfWork uow)
        {
            _uow ??= uow;
            _budgetManager ??= new BudgetManager(_uow);
            _logger = log;
        }

        [FunctionName("CreateBudget")]
        [OpenApiOperation(operationId: "CreateBudget", tags: new[] { "Budget CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(string), Description = "Add Budget")]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateBudget(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Budget/Create")] HttpRequest req,
        [AccessToken] ClaimsPrincipal principal)
        {
            _logger.LogInformation("Creating a new budget.");

            // Extract userId from the token
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation($"userId: {userId}");
            if (string.IsNullOrEmpty(userId))
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var budget = JsonConvert.DeserializeObject<DAL.Model.Budget>(requestBody);

            // Validate month input
            if (budget.Month < 1 || budget.Month > 12)
            {
                return new BadRequestObjectResult("Month must be between 1 and 12.");
            }

            // Check if there is already a budget for the same month and year
            var existingBudget = await _budgetManager.GetBudgetByMonthAndYearAsync(budget.Month, budget.Year, _logger);
            if (existingBudget != null)
            {
                return new BadRequestObjectResult("A budget already exists for the same month and year.");
            }

            // Assign the userId to the budget
            budget.UserId = userId;

            var createdBudget = await _budgetManager.CreateBudget(budget, _logger, userId);

            return new OkObjectResult(createdBudget);
        }

        [FunctionName("GetBudgetById")]
        [OpenApiOperation(operationId: "GetBudgetById", tags: new[] { "Budget CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiParameter(name: "budgetId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The BudgetId")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DAL.Model.Budget), Description = "The Budget object")]
        public async Task<IActionResult> GetBudgetById(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Budget/{budgetId}")] HttpRequest req,
           string budgetId,
           [AccessToken] ClaimsPrincipal principal
         )
        {
            _logger.LogInformation($"Getting budget with ID: {budgetId}");

            var result = await _budgetManager.GetBudgetById(budgetId, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("GetBudgets")]
        [OpenApiOperation(operationId: "GetBudgets", tags: new[] { "Budget CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiParameter(name: "CT", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Continuation Token")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<BudgetDTO>), Description = "The OK response")]
        public async Task<IActionResult> GetUsers(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Budget")] HttpRequest req,
           [AccessToken] ClaimsPrincipal principal
           )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string continuationToken = req.Query["CT"];

            var result = await _budgetManager.GetAllBudget(continuationToken, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("UpdateBudget")]
        [OpenApiOperation(operationId: "UpdateBudget", tags: new[] { "Budget CRUD" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(BudgetDTO), Description = "Budget DTO Update")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> UpdateBudget(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Budget/Update/{budgetId}")] HttpRequest req,
        string budgetId,
        [AccessToken] ClaimsPrincipal principal
        )
        {
            _logger.LogInformation($"Updating budget with ID: {budgetId}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedBudget = JsonConvert.DeserializeObject<DAL.Model.Budget>(requestBody);

            var result = await _budgetManager.UpdateBudget(budgetId, updatedBudget, _logger);

            return new OkObjectResult(result);
        }

        [FunctionName("DeleteBudget")]
        [OpenApiOperation(operationId: "DeleteBudget", tags: new[] { "Budget CRUD" })]
        [OpenApiParameter(name: "budgetId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The BudgetId")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteBudget(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Budget/Delete/{budgetId}")] HttpRequest req,
        string budgetId,
        [AccessToken] ClaimsPrincipal principal
        )
        {
            _logger.LogInformation($"Deleting budget with ID: {budgetId}");

            await _budgetManager.DeleteBudget(budgetId, _logger);

            return new OkResult();
        }

        [FunctionName("GetBudgetsByUserId")]
        [OpenApiOperation(operationId: "GetBudgetsByUserId", tags: new[] { "Budget CRUD" })]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The User ID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<BudgetDTO>), Description = "The list of Budgets for the User")]
        public async Task<IActionResult> GetBudgetsByUserId(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Budget/ByUserId/{userId}")] HttpRequest req,
        string userId,
        [AccessToken] ClaimsPrincipal principal
        )
        {
            _logger.LogInformation($"Getting budgets for user with ID: {userId}");

            // Fetch budgets for the specified user ID
            var budgets = await _budgetManager.GetBudgetsByUserId(userId, _logger);

            // Convert to DTO
            var budgetDTOs = new List<BudgetDTO>();
            foreach (var budget in budgets)
            {
                budgetDTOs.Add(new BudgetDTO
                {
                    Month = budget.Month,
                    Year = budget.Year,
                    Amount = budget.Amount,
                    UserId = budget.UserId
                });
            }

            return new OkObjectResult(budgetDTOs);
        }

        [FunctionName("GetBudgetByMonthAndYear")]
        [OpenApiOperation(operationId: "GetBudgetByMonthAndYear", tags: new[] { "Budget CRUD" })]
        [OpenApiParameter(name: "month", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The month")]
        [OpenApiParameter(name: "year", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The year")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<DAL.Model.Budget>), Description = "The list of Budget objects")]
        public async Task<IActionResult> GetBudgetByMonthAndYear(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Budget/ByMonthAndYear/{month}/{year}")] HttpRequest req,
        int month,
        int year,
        [AccessToken] ClaimsPrincipal principal
        )
        {
            _logger.LogInformation($"Getting list of budgets for month: {month}, year: {year}.");

            var budgets = await _budgetManager.GetBudgetByMonthAndYearAsync(month, year, _logger);

            return new OkObjectResult(budgets);
        }
        public async Task<DAL.Model.Budget> GetBudgetByMonthAndYearAsync(int month, int year, ILogger log)
        {
            // Implementasi untuk mendapatkan anggaran berdasarkan bulan dan tahun
            var result = await _uow.BudgetRepository.GetAsync(
                predicate: budget => budget.Month == month && budget.Year == year);

            return result?.Items.FirstOrDefault();
        }

    }
}

