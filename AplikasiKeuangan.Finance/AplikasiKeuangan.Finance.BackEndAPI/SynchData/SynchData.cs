using AplikasiKeuangan.Expenses.BLL.Report;
using AplikasiKeuangan.Finance.BLL.BudgetManagement;
using AplikasiKeuangan.Finance.BLL.ExpensesCategoryManagement;
using AplikasiKeuangan.Finance.BLL.ExpensesManagement;
using AplikasiKeuangan.Finance.DAL;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AplikasiKeuangan.Finance.BackEndAPI.SynchData
{
    public class SynchReportData
    {
        private readonly IUnitOfWork _uow;
        private readonly ReportManager _manager;
        private readonly BudgetManager _budgetManager;
        private readonly ExpensesManager _expensesManager;
        private readonly ExpensesCategroyManager _expensesCategoryManager;

        public SynchReportData(IUnitOfWork uow)
        {
            _uow ??= uow;
            _manager = new ReportManager(_uow);
            _budgetManager = new BudgetManager(_uow);
            _expensesManager = new ExpensesManager(_uow);
            _expensesCategoryManager = new ExpensesCategroyManager(_uow);
        }

        [FunctionName("SynchDataReport")]
        public async Task Run(
            [EventHubTrigger(
                "evh-rizky-aplikasi-keuangan",
                Connection = "evh-rizky-aplikasi-keuangan",
                ConsumerGroup = "report-synchreport")] EventData[] events,
            ILogger log
            )
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    var evgBody = eventData.EventBody.ToString();
                    var evgDatas = JsonConvert.DeserializeObject
                        <List<EventGridEvent>>(evgBody);

                    foreach (var evgData in evgDatas)
                    {
                        var report = JsonConvert.DeserializeObject
                            <DAL.Model.Report>(evgData.Data.ToString());
                        switch (evgData.Subject)
                        {
                            case "Update/":
                                var generatedReport = await _manager.GenerateMonthlyReportAsync(report.Month, report.Year);
                                log.LogWarning($"data monthly {generatedReport}");
                                break;

                            case "Delete/":
                                log.LogWarning($"Delete data");
                                break;
                        }
                    }
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}