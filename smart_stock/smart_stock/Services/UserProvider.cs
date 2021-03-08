using System.Linq.Expressions;
using BC = BCrypt.Net.BCrypt;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<bool> GetUserCredential(string username)
        {
            using (MySqlConnection connection = Connection)
            {
                var @credParam = new { username = username };
                Connection.Open();
                string existingUser = await connection.QueryFirstOrDefaultAsync<string>("SELECT username FROM Credential where username = @username", credParam);
                if (existingUser == null)
                {
                    return false;
                }
                return true;

            }

        }

        public async Task<User> InsertUser(User user)
        {   
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {              
                    // Add entry to Crential table  
                    var sQuery = @"INSERT INTO Credential (username, password) VALUES (@username, @password)";        
                    var @params = new {
                        username = user.Credential.Username,
                        password = user.Credential.Password
                        
                    };      
                    connection.Open();
                    result = await connection.ExecuteAsync(sQuery, @params);
                    // Add entry to PII table  
                    if (result > 0)
                    {
                        sQuery = @"INSERT INTO PII (fname, lname, dob, email, phone) VALUES (@FName, @LName, @dob, @email, @phone)";        
                        var @params2 = new {                            
                            Fname = user.Pii.FName,
                            LName = user.Pii.LName,
                            dob   = user.Pii.Dob  ,
                            email = user.Pii.Email,
                            phone = user.Pii.Phone
                        };      
                        result = await connection.ExecuteAsync(sQuery, @params2);
                    }
                    // Add entry to User table  
                    if (result > 0)
                    {                 
                        int piiId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM PII ORDER BY id DESC LIMIT 1", null);       
                        int credId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM Credential ORDER BY id DESC LIMIT 1", null);
                        sQuery = @"INSERT INTO User (pii, credentials, joindate, dateadded, dateconfirmed, alpacakeyid, alpacakey) VALUES (@pii, @credentials, @joinDate, @dateAdded, @dateConfirmed, @alpacaKeyId, @alpacaKey)";        
                        var @params3 = new {
                            pii           = piiId ,
                            credentials   = credId,
                            joinDate      = user  .JoinDate,
                            dateAdded     = user  .DateAdded,
                            dateConfirmed = user  .DateConfirmed,
                            alpacaKeyId = user.AlpacaKeyId,
                            alpacaKey = user.AlpacaKey
                        };      
                        result = await connection.ExecuteAsync(sQuery, @params3);
                        if (result > 0)
                        {                            
                            string userQuery = "SELECT id, joindate, dateadded, dateconfirmed, alpacakeyid, alpacakey FROM User WHERE pii = @piiId AND credentials = @credentialId";
                            var @userParams = new {
                                piiId        = piiId ,
                                credentialId = credId
                            };
                            User newUser = await connection.QueryFirstOrDefaultAsync<User>(userQuery, @userParams);

                            // Add entry to Portfolio table
                            var portQuery = @"INSERT INTO Portfolio (User, Amount, Profit, Loss, Net, Invested, Cash) VALUES (@user, @amount, @profit, @loss, @net, @invested, @cash)";
                            var @paramsPort = new {
                                user     = newUser.Id,
                                amount   = 0,
                                profit   = 0,
                                loss     = 0,
                                net      = 0,
                                invested = 0,
                                cash     = 0
                            };
                            result = -1;
                            result = await connection.ExecuteAsync(portQuery, @paramsPort);                            
                            
                            if (result > 0) return newUser;
                        }
                    }
                }
                return null;
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }            
        }

        public async Task<User> GetAllUserInformation(int userId, string username)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string credIdQuery = "SELECT Id, Username FROM Credential WHERE Username = @username";
                    var @idParam = new {
                        Username = username
                    };
                    connection.Open();
                    Credential credential = await connection.QueryFirstOrDefaultAsync<Credential>(credIdQuery, idParam);
                    if (credential == null)
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
                    return user;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<AlpacaSecret> GetUserAlpacaKeys(int userId)
        {
            try
            {
                using(MySqlConnection connection = Connection)
                {
                    string keyQuery = "SELECT AlpacaKeyId, AlpacaKey FROM User WHERE Id = @userId";
                    var @keyParam = new {userId = userId};
                    AlpacaSecret secret = await connection.QueryFirstOrDefaultAsync<AlpacaSecret>(keyQuery, @keyParam);
                    return secret;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(TAG + e);
                return null;
            }
        }
    }
}