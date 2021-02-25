using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface IUserProvider
    {
        Task<User> GetUserLogin(string username, string password);

        /*
        Queries credential table for duplicate usernames, really only used for conflicts. 
        */  
        Task<bool> GetUserCredential(string username);

        /* Insert a single user into table 'User'.
            Insert a single user's credentials into table 'Credential'.
            Insert a single user's pii into the table 'PII'
            returns true on success given the parameter 'user' */
        Task<User> InsertUser(User user);

        /* Delete a user from table tables 'User', 'Credentials', and 'PII'.
            Returns true on success given the parameter 'user' */
        
    }
}