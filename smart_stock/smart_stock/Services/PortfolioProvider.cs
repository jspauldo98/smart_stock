using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
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

        public async Task<Portfolio> GetPortfolio(int id)
        {   
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT Id, Profit, Loss, Net FROM Portfolio p WHERE p.User = @userId";
                    var @param = new {userId = id};
                    connection.Open();
                    Portfolio p = await connection.QueryFirstOrDefaultAsync<Portfolio>(sQuery, @param);

                    sQuery = "SELECT Id, JoinDate, DateAdded, DateConfirmed FROM User WHERE ID = @id";
                    var @param2 = new {id = id };
                    p.User = await connection.QueryFirstOrDefaultAsync<User>(sQuery, @param2);  

                    string piiQuery = "SELECT Pii FROM User Where ID = @id";
                    var @piiParam = new {id = id};
                    int? pii = await connection.QueryFirstOrDefaultAsync<int?>(piiQuery, @piiParam);
                    
                    sQuery = "SELECT Id, FName, LName, Dob, Email, Phone FROM PII WHERE Id = @piiId";
                    var @param3 = new {piiid = pii};
                    p.User.Pii = await connection.QueryFirstOrDefaultAsync<Pii>(sQuery, @param3);

                    string credQuery = "SELECT Credentials FROM User Where ID = @id";
                    var @credParam = new {id = id};
                    int? cred = await connection.QueryFirstOrDefaultAsync<int?>(credQuery, @credParam);

                    sQuery = "SELECT Id, Username FROM Credential WHERE Id = @credid";
                    var @param4 = new {credid = cred};
                    p.User.Credential = await connection.QueryFirstOrDefaultAsync<Credential>(sQuery, @param4);

                    return p;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<IEnumerable<TradeAccount>> GetTradeAccounts(int pId)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT ta.Id, ta.Title, ta.Profit, ta.Loss, ta.Net, ta.NumTrades, ta.NumSTrades, ta.NumFTrades, ta.DateCreated, ta.DateModified FROM TradeAccount ta, Portfolio p WHERE p.Id = ta.Portfolio and p.Id = @id";
                    var @param = new {id = pId};
                    connection.Open();
                    var tas = await connection.QueryAsync<TradeAccount>(sQuery, @param);
                    // Assign prefferences for each trade account
                    foreach (TradeAccount t in tas)
                    {                
                        // get preference Id    
                        var prefIdQ = "SELECT Preference FROM TradeAccount WHERE Id = @id";
                        var @prefIdParam = new {id = t.Id};   
                        int prefID = await connection.QueryFirstOrDefaultAsync<int>(prefIdQ, @prefIdParam);
                        // get risk level id
                        var riskIdQ = "SELECT RiskLevel FROM Preference WHERE Id = @pId";
                        var @riskIdParam = new {pId = prefID};
                        int riskId = await connection.QueryFirstOrDefaultAsync<int>(riskIdQ, @riskIdParam);
                        // get risk level
                        var riskQ = "SELECT Id, Risk, DateAdded FROM RiskLevels WHERE Id = @rId";
                        var @riskParam = new {rId = riskId};
                        RiskLevel riskLevel = await connection.QueryFirstOrDefaultAsync<RiskLevel>(riskQ, @riskParam);
                        // get preference
                        var prefQ = "SELECT Id, DateModified, CapitalToRisk FROM Preference WHERE Id = @pId";
                        var @prefParam = new {pId = prefID};
                        t.Preference = await connection.QueryFirstOrDefaultAsync<Preference>(prefQ, @prefParam);
                        t.Preference.RiskLevel = riskLevel;
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

        // TODO - right now this is just generating a template trade account. In future get rid of portfolio id param and use real trade account object 
        public async Task<bool> InsertTradeAccount(int portfolioId)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {       
                    // Add entry to Preference
                    var prefQ = @"INSERT INTO Preference (RiskLevel, CapitalToRisk) VALUES (@risk, @capital)";        
                    var @prefP = new {
                        risk = 1,
                        capital = 10
                    };      
                    connection.Open();
                    result = await connection.ExecuteAsync(prefQ, @prefP);

                    // REtrieve preference id
                    int prefId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM PII ORDER BY id DESC LIMIT 1", null); 

                    // Add entry to TradeStratiegies
                    var stratQ = @"INSERT INTO TradeStrategies (Preference, BlueChip, LongTerm, Swing, Scalp, Day, DateAdded) VALUES (@pref, @bluechip, @longterm, @swing, @scalp, @day, @dateAdded)";        
                    var @stratP = new {
                        pref = prefId,
                        bluechip = 1,
                        longterm = 0,
                        swing = 1,
                        scalp = 0,
                        day = 0,
                        dateAdded = DateTime.Now
                    };      
                    result = await connection.ExecuteAsync(stratQ, @stratP);   
                    
                    // Add entry to TradeAccount         
                    var tradeQ = @"INSERT INTO TradeAccount (Portfolio, Preference, Title, Profit, Loss, Net, NumTrades, NumSTrades, NumFTrades, DateCreated) VALUES (@port, @pref, @title, @profit, @loss, @net, @numTrades, @numSTrades, @numFTrades, @dateCreated)";        
                    var @tradeP = new {
                        port = portfolioId,
                        pref = prefId,
                        title = "User Created Account",
                        profit = 0,
                        loss = 0,
                        net = 0,
                        numTrades = 0,
                        numSTrades = 0,
                        numFTrades = 0,
                        dateCreated = DateTime.Now
                    };      
                    result = await connection.ExecuteAsync(tradeQ, @tradeP);
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