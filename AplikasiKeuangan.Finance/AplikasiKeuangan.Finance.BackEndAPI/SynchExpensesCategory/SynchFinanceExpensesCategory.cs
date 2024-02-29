using Azure.Messaging.EventHubs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikasiKeuangan.Finance.BackEndAPI.SynchExpensesCategory
{
    public class SynchFinanceExpensesCategory
    {
        [FunctionName("SynchFinanceExpenses")]
        public static async Task Run([EventHubTrigger("evh-aplikasi-keuangan",
            Connection = "evh-aplikasi-keuangan", ConsumerGroup ="report.expensescategory-synchexpensescategory")]
        EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    // Replace these two lines with your processing logic.
                    log.LogInformation($"Event Body Category: {eventData.EventBody}");

                    var evgData = JsonConvert.DeserializeObject<List<EventGridEvent>>(eventData.EventBody.ToString());
                    foreach (var item in evgData)
                    {
                        //switch (item.Subject)
                        //{
                        //    case "Create/":
                        //        var data = JsonConvert.DeserializeObject<DAL.Model.Expenses>(item.Data.ToString());
                        //        log.LogInformation($"Expenses category has been created: {data}");
                        //        break;
                        //    case "Update/":
                        //        var updatedData = JsonConvert.DeserializeObject<DAL.Model.Expenses>(item.Data.ToString());
                        //        log.LogInformation($"Expenses category has been updated: {updatedData}");
                        //        break;
                        //    case "Delete/":
                        //        log.LogInformation("Expenses category has been deleted");
                        //        break;
                        var jadiModelProfile = JsonConvert.DeserializeObject<DAL.Model.Expenses>(item.Data.ToString());
                        log.LogInformation($"C# Event Hub trigger function processed a message: {jadiModelProfile}");
                        //}
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
