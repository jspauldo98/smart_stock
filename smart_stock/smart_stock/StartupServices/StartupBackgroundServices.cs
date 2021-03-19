using System.Collections.Generic;
using System.Threading;
using System;
using System.Collections;
using System.Threading.Tasks;
using smart_stock.Services;
using smart_stock.Models;
using smart_stock.AlpacaServices;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using Dapper;

namespace smart_stock.StartupServices
{
    public class StartupBackgroundServices : IStartupBackgroundServices
    {
        private readonly IConfiguration _config;

        //This class does it's own querying as it cannot rely on any other provider because it is called
        //directly in Startup.cs         
        public StartupBackgroundServices(IConfiguration config)
        {
            _config = config;
            StartTradingFilesForAllUsers();
        }

        public MySqlConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async void StartTradingFilesForAllUsers()
        {
            try
            {
                // First get users alpaca data and trade accounts assigned to a tuple to run in parallel later
                List<(AlpacaSecret, TradeAccount[])> users = await GetUserData();

                // Now execute trading for all users in parallel
                // TODO FOR NOW BREAK AFTER TWO!!!! OTHERWISE PREPARE FOR ANAL ABLITERATION
                Parallel.For(0, users.Count, i => 
                {
                    if (i > 1) return;
                    Trading trader = new Trading(users[i].Item1, users[i].Item2);
                });
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("executed properly");
        }  

        private async Task<List<(AlpacaSecret, TradeAccount[])>> GetUserData()
        {
            List<(AlpacaSecret, TradeAccount[])> users = new List<(AlpacaSecret, TradeAccount[])>();
            using(MySqlConnection connection = Connection)
            {
                string usersQuery = "SELECT Id FROM User";
                connection.Open();
                var userIds = (await connection.QueryAsync<int>(usersQuery, null)).ToArray();                    
                foreach(var id in userIds)
                {
                    string keyQuery = "SELECT AlpacaKeyId, AlpacaKey FROM User WHERE Id = @id";
                    var @keyParam = new {id = id};
                    AlpacaSecret userAlpacaSecret = await connection.QueryFirstOrDefaultAsync<AlpacaSecret>(keyQuery, keyParam);

                    string portfolioQuery = "SELECT Id FROM Portfolio WHERE User = @id";
                    var @portfolioParam = new {id = id};
                    int portfolioId = await connection.QueryFirstOrDefaultAsync<int>(portfolioQuery, portfolioParam);

                    string tradeAccountsQuery = "SELECT Id, Title, Description, Amount, Profit, Loss, Net, NumTrades, NumSTrades, NumFTrades, DateCreated, DateModified, Invested, Cash FROM TradeAccount WHERE Portfolio = @id";
                    var @tradeAccountParam = new {id = portfolioId};
                    TradeAccount[] userTradeAccountsList = (await connection.QueryAsync<TradeAccount>(tradeAccountsQuery, tradeAccountParam)).ToArray();
                    foreach(var tradeAccount in userTradeAccountsList)
                    {
                        string preferenceIdQuery = "SELECT Preference FROM TradeAccount WHERE Id = @id";
                        var @preferenceIdParam = new {id = tradeAccount.Id};
                        int preferenceId = await connection.QueryFirstOrDefaultAsync<int>(preferenceIdQuery, preferenceIdParam);

                        string tradeStrategyIdQuery = "SELECT TradeStrategy FROM Preference WHERE Id = @id";
                        var @tradeStrategyIdParam = new {id = preferenceId};
                        int tradeStrategyId = await connection.QueryFirstOrDefaultAsync<int>(tradeStrategyIdQuery, tradeStrategyIdParam);

                        string sectorIdQuery = "SELECT Sector FROM Preference WHERE Id = @id";
                        var @sectorIdParam = new {id = preferenceId};
                        int sectorId = await connection.QueryFirstOrDefaultAsync<int>(sectorIdQuery, sectorIdParam);

                        Sector sector = await connection.QueryFirstOrDefaultAsync<Sector>("SELECT * FROM Sectors WHERE Id = @id", new {id = sectorId});

                        TradeStrategy strategy = await connection.QueryFirstOrDefaultAsync<TradeStrategy>("SELECT * FROM TradeStrategies WHERE Id = @id", new {id = tradeStrategyId});

                        Preference preference = await connection.QueryFirstOrDefaultAsync<Preference>("SELECT Id, CapitalToRisk FROM Preference WHERE Id = @id", new {id = preferenceId});

                        preference.Sector = sector;
                        
                        preference.TradeStrategy = strategy;

                        tradeAccount.Preference = preference;

                        //My code flow so good Buckner would cry. To bad the time complexity of this is kind of shit. 
                    }
                    users.Add((userAlpacaSecret, userTradeAccountsList));
                }                    
            }
            return users;
        } 
    } 
}