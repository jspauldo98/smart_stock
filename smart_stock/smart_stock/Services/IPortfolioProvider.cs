using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface IPortfolioProvider
    {       
        /* Get portfolio from table 'Portfolio' given user object.
            Get User from table 'User' given user object 
            Returns a Portfolio object */
        Task<Portfolio> GetPortfolio(int id);

        /* Get all Trade Accounts from table 'TradeAccount" given portfolio object
            Returns a list of TradeAccount objects that correspond to portfolio object */
        Task<IEnumerable<TradeAccount>> GetTradeAccounts(int pId);

        /* Update a single trade account in table 'TradeAccount' given a trade account object.
            Returns true on success given parameter 'ta' and 'id' */
        Task<bool> UpdateTradeAccount(TradeAccount ta, int id);

        /* Insert a single trade account into table 'TradeAccount'
            Insert a single preference object into table 'Preference'
            Insert a single trade strategy object into table 'TradeStrategies'
            returns true on success given the parameter 'ta' representing a trade account */
        Task<bool> InsertTradeAccount(TradeAccount ta);

        /* Check if a trade account exists. Checks from table 'TradeAccount'.
            Returns true on success given the parameter 'id' */
        bool TradeAccountExists(int id);
    }
}