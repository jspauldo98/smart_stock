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
    public class LogProvider : ILogProvider
    {
        private readonly IConfiguration _config;
        private readonly string TAG = "LogProvider";
        private int _accountId;
        public LogProvider(IConfiguration config)
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

        public async Task<bool> RecordTradeInLog(int tradeId)
        {
            try 
            {
                using(MySqlConnection connection = Connection)
                {
                    string updateQuery = "INSERT INTO Log (Trade) VALUES (@TradeId) WHERE TradeAccount = @TradeAccount";
                    var @updateParams = new {TradeId = tradeId, TradeAccount = _accountId};
                    connection.Open();
                    await connection.ExecuteAsync(updateQuery, updateParams);
                    return true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(TAG + e);
                return false;
            }
        }

        public void setGlobalAccountId(int accountId)
        {
            _accountId = accountId;
        }
        public async Task<IEnumerable<Log>> GetLog(int tId)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT Id, Date, TradeAccountAmount, PortfolioAmount FROM Log WHERE TradeAccount=@id";
                    var @param = new {id = tId};
                    connection.Open();
                    var logs = await connection.QueryAsync<Log>(sQuery, @param);
                    // Assign Trades for each Log
                    foreach (Log l in logs)
                    {                
                        // get trade id
                        string tradeIdQ = "SELECT Trade FROM Log WHERE Id=@id";
                        var @tradeIdP = new {id = l.Id};
                        var tradeId = await connection.QueryFirstOrDefaultAsync<int>(tradeIdQ, @tradeIdP);

                        // get trade based on trade id
                        string tradeQ = "SELECT Id, Type, Ticker, Amount, Price, Quantity, Date FROM Trade WHERE Id=@id";
                        var @tradeP = new {id = tradeId};
                        var trade = await connection.QueryFirstOrDefaultAsync<Trade>(tradeQ, @tradeP);

                        // assign trade to log
                        l.Trade = trade;
                    }
                    return logs.ToList();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }
    }
}
