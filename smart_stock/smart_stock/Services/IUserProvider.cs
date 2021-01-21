using System.Collections.Generic;
using System.Threading.Tasks;
using smart_stock.Models;

namespace smart_stock.Services
{
    public interface IUserProvider
    {
        /* Get all users from table UserBase */
        Task<IEnumerable<UserBase>> GetAllUsers();

        /* Get a single user from table UserBase given a user id.
            Returns a list of UserBase objects on success */
        Task<UserBase> GetUser(int id);

        /* Update a single user in table UserBase given a user id.
            Returns UserBase object on success given parameter 'id' */
        Task<bool> UpdateUser(int id, UserBase user);

        /* Insert a single user into table UserBase.
            returns true on success given the parameter 'user' */
        Task<bool> InsertUser(UserBase user);

        /* Delete a user from table tables UserBase and User
            Returns true on success given the parameter 'user' */
        Task<bool> DeleteUser(int id);

        /* Check if a user exists. Checks from table UserBase.
            Returns true on success given the parameter 'id' */
        bool UserExists(int id);
    }
}