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
        
        /*
        Once a user has been properly logged in and set up with a claim, they will be able to access this
        method and retrieve all their information as needed
        */
        Task<User> GetAllUserInformation(int userId, string username);

        /*
            Exclusively used for alpaca API key retrieval based on user ID        
        */
        Task<AlpacaSecret> GetUserAlpacaKeys(int userId);
    }
}