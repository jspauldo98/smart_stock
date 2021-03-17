using System.Threading.Tasks;
using System.Collections.Generic;
using smart_stock.Models;

namespace smart_stock.AlpacaServices
{
    public interface ITrading
    {
        void Start(AlpacaSecret secret, IEnumerable<TradeAccount> tradeAccounts);
        void Dispose();
    }
}