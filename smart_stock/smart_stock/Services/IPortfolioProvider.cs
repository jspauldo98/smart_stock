using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface IPortfolioProvider
    {
        /* Insert a single portfolio into table 'Portfolio'
            Returns true on success given parameter 'portfolio */
        Task<bool> InsertPortfolio(Portfolio portfolio);

        /* Get portfolio from table 'Portfolio' given user object.
            Get User from table 'User' given user object 
            Returns a Portfolio object */
        Task<Portfolio> GetPortfolio(User u);

        /* Update a single user in table 'Portfolio' given a portfolio id.
            Returns User object on success given parameter 'id' and 'p' */
        Task<bool> UpdatePortfolio(int id, Portfolio p);

        /* Get all Trade Accounts from table 'TradeAccount" given portfolio object
            Returns a list of TradeAccount objects that correspond to portfolio object */
        Task<IEnumerable<TradeAccount>> GetTradeAccounts(Portfolio p);

        /* Delete a portfolio from table 'Portfolio'.
            Returns true on success given the parameter of portfolio id */
        Task<bool> DeleteUser(int id);

        /* Check if a portfolio exists. Checks from table 'Portfolio'.
            Returns true on success given the parameter 'id' */
        bool PortfolioExists(int id);
    }
}