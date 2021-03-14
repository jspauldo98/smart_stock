using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface ILogProvider
    {
        Task<bool> RecordTradeInLog(int tradeId);
        void setGlobalAccountId(int accountId);
        /* Get all log entries from table 'Log' given a trade account object id
            Return all log objects associated with trade account */
        Task<IEnumerable<Log>> GetLog(int tId);
    }
}
