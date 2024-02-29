using AplikasiKeuangan.Expenses.BLL.Report;
using AplikasiKeuangan.Finance.DAL;
using Microsoft.Extensions.Logging;

namespace AplikasiKeuangan.Finance.FrontEndAPI.Report
{
    public class Report
    {
        private readonly ILogger<Report> _logger;
        private readonly IUnitOfWork _uow;
        private readonly ReportManager _reportManager;

        public Report(ILogger<Report> log, IUnitOfWork uow)
        {
            _uow ??= uow;
            _reportManager ??= new ReportManager(_uow);
            _logger = log;
        }
    }
}
