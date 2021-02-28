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
                    string sQuery = "SELECT ta.Id, ta.Title, ta.Description, ta.Amount, ta.Profit, ta.Loss, ta.Net, ta.NumTrades, ta.NumSTrades, ta.NumFTrades, ta.Invested, ta.Cash, ta.DateCreated, ta.DateModified FROM TradeAccount ta, Portfolio p WHERE p.Id = ta.Portfolio and p.Id = @id";
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
                        // get strat id
                        var stratIdQ = "SELECT TradeStrategy FROM Preference WHERE Id = @pId";
                        var @stratIdParam = new {pId = prefID};
                        int stratId = await connection.QueryFirstOrDefaultAsync<int>(stratIdQ, @stratIdParam);
                        // get trade strat
                        var stratQ = "SELECT Id, BlueChip, LongTerm, Swing, Scalp, Day FROM TradeStrategies WHERE Id = @sId";
                        var @stratParam = new {sId = stratId};
                        TradeStrategy strategy = await connection.QueryFirstOrDefaultAsync<TradeStrategy>(stratQ, @stratParam);
                        // get sector id
                        var secIdQ = "SELECT Sector FROM Preference WHERE Id = @pId";
                        var @secIdParam = new {pId = prefID};
                        int secId = await connection.QueryFirstOrDefaultAsync<int>(secIdQ, @secIdParam);
                        // get sector
                        var secQ = "SELECT InformationTechnology, HealthCare, Financials, ConsumerDiscretionary, Communication, Industrials, ConsumerStaples, Energy, Utilities, RealEstate, Materials FROM Sectors WHERE Id = @secId";
                        var @secParam = new {secId = secId};
                        Sector sector = await connection.QueryFirstOrDefaultAsync<Sector>(secQ, @secParam);
                        // get preference
                        var prefQ = "SELECT Id, CapitalToRisk FROM Preference WHERE Id = @pId";
                        var @prefParam = new {pId = prefID};
                        t.Preference = await connection.QueryFirstOrDefaultAsync<Preference>(prefQ, @prefParam);
                        t.Preference.RiskLevel = riskLevel;
                        t.Preference.TradeStrategy = strategy;
                        t.Preference.Sector = sector;
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

        // TODO - right now this is just generating a template trade account. In future get rid of portfolio id param and use real trade account object 
        public async Task<bool> InsertTradeAccount(int portfolioId)
        {
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                                                
                    // Add entry to TradeStratiegies
                    var stratQ = @"INSERT INTO TradeStrategies (BlueChip, LongTerm, Swing, Scalp, Day) VALUES (@bluechip, @longterm, @swing, @scalp, @day)";        
                    var @stratP = new {
                        bluechip = true,
                        longterm = true,
                        swing    = true,
                        scalp    = true,
                        day      = true,
                    };      
                    result = await connection.ExecuteAsync(stratQ, @stratP); 

                    // REtrieve strat id
                    int stratId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM TradeStrategies ORDER BY id DESC LIMIT 1", null);

                    // Add entry to Sectors
                    var secQ = "INSERT INTO Sectors (InformationTechnology, HealthCare, Financials, ConsumerDiscretionary, Communication, Industrials, ConsumerStaples, Energy, Utilities, RealEstate, Materials) VALUES (@infoTech, @healthCare, @fin, @consumeD, @comm, @indust, @consumS, @energy, @util, @realE, @mat)";
                    var @secP = new {
                        infoTech   = true,
                        healthCare = true,
                        fin        = true,
                        consumeD   = true,
                        comm       = true,
                        indust     = true,
                        consumS    = true,
                        energy     = true,
                        util       = true,
                        realE      = true,
                        mat        = true
                    };    
                    result = await connection.ExecuteAsync(secQ, @secP); 

                    // REtrieve sector id
                    int sectorId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM Sectors ORDER BY id DESC LIMIT 1", null);

                    // Add entry to Preference
                    var prefQ = @"INSERT INTO Preference (RiskLevel, TradeStrategy, Sector, CapitalToRisk) VALUES (@risk, @strat, @sector, @capital)";        
                    var @prefP = new {
                        risk    = 1,
                        strat   = stratId,
                        sector  = sectorId,
                        capital = 10
                    };      
                    connection.Open();
                    result = await connection.ExecuteAsync(prefQ, @prefP);  

                    // REtrieve preference id
                    int prefId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM Preference ORDER BY id DESC LIMIT 1", null);
                    
                    // Add entry to TradeAccount         
                    var tradeQ = @"INSERT INTO TradeAccount (Portfolio, Preference, Title, Description, Amount, Profit, Loss, Net, NumTrades, NumSTrades, NumFTrades, Invested, Cash, DateCreated) VALUES (@port, @pref, @title, @desc, @amount, @profit, @loss, @net, @numTrades, @numSTrades, @numFTrades, @invested, @cash, @dateCreated)";        
                    var @tradeP = new {
                        port        = portfolioId,
                        pref        = prefId,
                        title       = "User Created Account",
                        desc        = "This account is generated as a template for now.",
                        amount      = 0,
                        profit      = 0,
                        loss        = 0,
                        net         = 0,
                        numTrades   = 0,
                        numSTrades  = 0,
                        numFTrades  = 0,
                        invested    = 0,
                        cash        = 0,
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
    }
}