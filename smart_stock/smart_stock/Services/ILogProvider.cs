using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface ILogProvider
    {
        Task<bool> RecordTradeInLog(int tradeId);
        void setGlobalAccountId(int accountId);
    }
}
