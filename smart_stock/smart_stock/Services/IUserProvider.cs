using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface IUserProvider
    {
        /* Get all users from table 'User' */
        Task<IEnumerable<User>> GetAllUsers();

        /* Get a single user from table 'User' given a user id.
            Returns a list of User objects on success */
        Task<User> GetUser(int id);

        /* Retrieves a single user in table 'User' given a user id.
            Returns User object on success given parameter 'username' and 'password' */
        Task<User> GetUserLogin(string username, string password);  

        /* Update a single user in table 'User' given a user id.
            Returns User object on success given parameter 'id' and 'user' */  
        Task<bool> UpdateUser(int id, User user);

        /* Insert a single user into table 'User'.
            Insert a single user's credentials into table 'Credential'.
            Insert a single user's pii into the table 'PII'
            returns true on success given the parameter 'user' */
        Task<bool> InsertUser(User user);

        /* Delete a user from table tables 'User', 'Credentials', and 'PII'.
            Returns true on success given the parameter 'user' */
        Task<bool> DeleteUser(int id);

        /* Check if a user exists. Checks from table 'User'.
            Returns true on success given the parameter 'id' */
        bool UserExists(int id);
    }
}