using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface ITradeStrategiesProvider
    {
        //Interface to be used when creating a new tradeStrategy through the registration process
        Task<string> InsertTradeStrategy(TradeStrategy tradeStrategy);
    }
}