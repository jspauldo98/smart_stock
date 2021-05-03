using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface ILogProvider
    {
        Task<bool> RecordTradeInLog(Log log);
        
        Task<IEnumerable<Log>> GetLog(int tId);

        Task<IEnumerable<Log>> GetMinuteLog(int tId);

        Task<IEnumerable<Log>> GetHourLog(int tId);

        Task<IEnumerable<Log>> GetDayLog(int tId);
        Task<IEnumerable<Log>> GetWeekLog(int tId);

        Task<IEnumerable<Log>> GetMonthLog(int tId);

        Task<IEnumerable<Log>> GetYearLog(int tId);

        Task<IEnumerable<Log>> GetMinuteLogByPortfolio(int pId);

        Task<IEnumerable<Log>> GetHourLogByPortfolio(int pId);

        Task<IEnumerable<Log>> GetDayLogByPortfolio(int pId);
        Task<IEnumerable<Log>> GetWeekLogByPortfolio(int pId);

        Task<IEnumerable<Log>> GetMonthLogByPortfolio(int pId);

        Task<IEnumerable<Log>> GetYearLogByPortfolio(int pId);
    }
}
