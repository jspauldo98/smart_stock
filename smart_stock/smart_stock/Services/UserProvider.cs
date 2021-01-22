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
                return new MySqlConnection(_config.GetConnectionString("DevConnection"));
            }
        }
        public async Task<IEnumerable<UserBase>> GetAllUsers()
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT * FROM UserBase";
                    connection.Open();
                    var result = await connection.QueryAsync<UserBase>(sQuery);
                    return result.ToList();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<UserBase> GetUser(int id)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT id, user, date_added, date_confirmed FROM UserBase WHERE id = @id";
                    var @param = new {id = id };
                    connection.Open();
                    var result = await connection.QueryAsync<UserBase>(sQuery, @param);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<bool> UpdateUser(int id, UserBase user)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"UPDATE UserBase SET date_confirmed = @date_confirmed WHERE id = @id";        
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

        public async Task<bool> InsertUser(UserBase user)
        {   
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"INSERT INTO UserBase (user, date_added, date_confirmed) VALUES (@user, @date_added, @date_confirmed)";        
                    var @params = new {
                        user = user.user.Id,
                        date_added = user.DateAdded,
                        date_confirmed = user.DateConfirmed,
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

        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"DELETE userBase, user FROM UserBase INNERJOIN User WHERE userBase.id = @id and user.id = @id";     
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
                    var sQuery = @"SELECT EXISTS (SELECT * FROM UserBase WHERE id = @id)";     
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