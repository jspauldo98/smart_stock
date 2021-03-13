using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
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
    }
}
