using BC = BCrypt.Net.BCrypt;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using System.Linq;
using smart_stock.Models;


namespace smart_stock.Services
{
    public class UserProvider : IUserProvider
    {
        private readonly IConfiguration _config;
        private readonly string TAG = "UserProvider: ";
        public UserProvider(IConfiguration config)
        {
            _config = config;
        }

        public MySqlConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT * FROM User u JOIN Credential c on c.id = u.credentials JOIN PII p on p.id = u.pii";
                    connection.Open();
                    var result = await connection.QueryAsync<User>(sQuery);
                    return result.ToList();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<User> GetUser(int id)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT * FROM User u JOIN Credential c on c.id = u.credentials JOIN PII p on p.id = u.pii WHERE u.id = @id";
                    var @param = new {id = id };
                    connection.Open();
                    var result = await connection.QueryAsync<User>(sQuery, @param);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<User> GetUserLogin(string username, string password)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string credIdQuery = "SELECT * FROM Credential WHERE username = @username";
                    var @idParam = new {
                        username = username
                    };
                    connection.Open();
                    Credential credential = await connection.QueryFirstOrDefaultAsync<Credential>(credIdQuery, idParam);
                    if (credential == null)
                    {
                        return null;
                    }
                    bool isVerified = BC.Verify(password, credential.Password);
                    if (!isVerified)
                    {
                        return null;
                    }
                    string piiIdQuery = "SELECT pii FROM User WHERE credentials = @credentialId";
                    var @piiIdParam = new {credentialId = credential.Id};
                    int? piiId = await connection.QueryFirstOrDefaultAsync<int?>(piiIdQuery, @piiIdParam);

                    string piiQuery = "SELECT * FROM PII WHERE id = @id";
                    var @piiParam = new {id = piiId};
                    Pii pii = await connection.QueryFirstOrDefaultAsync<Pii>(piiQuery, @piiParam);

                    string userQuery = "SELECT id, joindate, dateadded, dateconfirmed FROM User WHERE pii = @piiId AND credentials = @credentialId";
                    var @userParams = new {
                        piiId = pii.Id,
                        credentialId = credential.Id
                    };
                    User user = await connection.QueryFirstOrDefaultAsync<User>(userQuery, @userParams);
                    user.Pii = pii;
                    user.Credential = credential;
                    password = null;
                    return user;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<bool> UpdateUser(int id, User user)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"UPDATE User SET date_confirmed = @date_confirmed WHERE id = @id";        
                    var @params = new {
                        join_date = user.DateConfirmed,
                        id = id
                    };      
                    connection.Open();
                    result = await connection.ExecuteAsync(sQuery, @params);
                }
                return result > 0;
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return false;
            }            
        }

        public async Task<bool> InsertUser(User user)
        {   
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"INSERT INTO Credentials (username, password) VALUES (@username, @password";        
                    var @params = new {
                        username = user.Credential.Username,
                        password = user.Credential.Password
                        
                    };      
                    connection.Open();
                    result = await connection.ExecuteAsync(sQuery, @params);
                    if (result > 0)
                    {
                        sQuery = @"INSERT INTO PII (f_name, l_name, dob, email, phone) VALUES (@f_name, @l_name, @dob, @email, @phone";        
                        var @params2 = new {                            
                            f_name = user.Pii.FName,
                            l_name = user.Pii.LName,
                            dob = user.Pii.Dob,
                            email = user.Pii.Email,
                            phone = user.Pii.Phone
                        };      
                        result = await connection.ExecuteAsync(sQuery, @params2);
                    }
                    if (result > 0)
                    {                        
                        sQuery = @"INSERT INTO User (pii, credentials, join_date, date_added, date_confirmed) VALUES (@pii, @credentials, @join_date, @date_added, @date_confirmed)";        
                        var @params3 = new {
                            pii = user.Pii.Id,
                            credentials = user.Credential.Id,
                            join_date = user.JoinDate,
                            date_added = user.DateAdded,
                            date_confirmed = user.DateConfirmed
                        };      
                        result = await connection.ExecuteAsync(sQuery, @params3);
                    }
                }
                return result > 0;
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return false;
            }            
        }

        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"DELETE u.*, c.*, p.* FROM User u JOIN Credential c on c.id = u.credentials JOIN PII p on p.id = u.pii WHERE user.id = @id";     
                    var @params = new { id = id };         
                    connection.Open();
                    result = await connection.ExecuteAsync(sQuery, @params);
                }
                return result > 0;
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return false;
            }
        }

        public bool UserExists(int id)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"SELECT EXISTS (SELECT * FROM User WHERE id = @id)";     
                    var @params = new { id = id };         
                    connection.Open();
                    result = connection.Query<int>(sQuery, @params).FirstOrDefault();
                }
                return result > 0;
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return false;
            }            
        }
    }
}