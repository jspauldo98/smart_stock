using System.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using System.Linq;
using smart_stock.Models;
using smart_stock.Services;

namespace smart_stock.Services
{
    public class PortfolioProvider : IPortfolioProvider
    {
        private readonly IConfiguration _config;
        private readonly string TAG = "PortfolioProvider";

        public PortfolioProvider(IConfiguration config)
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

        public async Task<bool> InsertPortfolio(Portfolio p)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    var sQuery = @"INSERT INTO Portfolio (User) VALUES (@user)";
                    var @params = new {
                        user = p.User.Id
                    };
                    connection.Open();
                    int result = -1;
                    result = await connection.ExecuteAsync(sQuery, @params);
                    return result > 0;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return false;
            }
        }

        public async Task<Portfolio> GetPortfolio(User u)
        {   
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT * FROM Portfolio p WHERE p.User = @userId";
                    var @param = new {userId = u.Id};
                    connection.Open();
                    Portfolio p = await connection.QueryFirstOrDefaultAsync<Portfolio>(sQuery, @param);
                    p.User = u;
                    return p;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<IEnumerable<TradeAccount>> GetTradeAccounts(Portfolio p)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT * FROM TradeAccount ta, Portfolio p WHERE p.Id = ta.Portfolio";
                    connection.Open();
                    IEnumerable<TradeAccount> tas = await connection.QueryAsync<TradeAccount>(sQuery);
                    foreach (TradeAccount t in tas)
                    {                       
                        t.Portfolio = p;
                    }
                    return tas.ToList();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<bool> UpdatePortfolio(int id, Portfolio p)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"UPDATE Portfolio SET Profit = @profit, Loss = @loss, Net = @net WHERE id = @id";        
                    var @params = new {
                        profit = p.Profit,
                        loss = p.Loss,
                        net = p.Net,
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

        public async Task<bool> DeletePortfolio(int id)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"DELETE * From Portfolio WHERE user.id = @id";     
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

        public bool PortfolioExists(int id)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var sQuery = @"SELECT EXISTS (SELECT * FROM Portfolio WHERE id = @id)";     
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