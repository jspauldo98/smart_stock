using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface ITradeProvider
    {
        Task<int> RecordTrade(Trade trade);
    }
}
