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

        /* Update a single user in table 'Portfolio' given a portfolio id.
            Returns User object on success given parameter 'id' and 'p' */
        Task<bool> UpdatePortfolio(int id, Portfolio p);

        /* Get all Trade Accounts from table 'TradeAccount" given portfolio object
            Returns a list of TradeAccount objects that correspond to portfolio object */
        Task<IEnumerable<TradeAccount>> GetTradeAccounts(int pId);

        /* Delete a portfolio from table 'Portfolio'.
            Returns true on success given the parameter of portfolio id */
        Task<bool> DeletePortfolio(int id);

        /* Insert a single trade account into table 'TradeAccount'
            Insert a single preference object into table 'Preference'
            Insert a single trade strategy object into table 'TradeStrategies'
            returns true on success given the parameter 't' representing a trade account */
        Task<bool> InsertTradeAccount(int portfolioId);    //TODO - right now this generates a template add real trade account in future and remove portfolio id as a param. Replace with trade account object

        /* Check if a portfolio exists. Checks from table 'Portfolio'.
            Returns true on success given the parameter 'id' */
        bool PortfolioExists(int id);
    }
}