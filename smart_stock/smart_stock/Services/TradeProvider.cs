using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using smart_stock.Models;


namespace smart_stock.Services
{
    public class TradeProvider : ITradeProvider
    {
        private readonly IConfiguration _config;
        private readonly string TAG = "TradeProvider";
        public TradeProvider(IConfiguration config)
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

        public async Task<bool> RecordTrade(Trade trade)
        {
            try
            {
                using(MySqlConnection connection = Connection) 
                {
                    string storeQuery = "INSERT INTO Trade (Ticker, Type, Amount, Price, Quantity, Date) values (@Id, @Ticker, @Type, @Amount, @Price, @Quantity, @Date)";
                    var @storeParams = new { Ticker = trade.Ticker, Type = trade.Type, Amount = trade.Amount, Price = trade.Price, Quantity = trade.Quantity, DateTime = trade.Date};
                    connection.Open();
                    await connection.ExecuteAsync(storeQuery, storeParams);
                    string idQuery = "SELECT Id FROM Trade WHERE Ticker = @Ticker AND Amount = @Amount";
                    var @idParams = new {Ticker = trade.Ticker, Amount = trade.Amount };
                    connection.Open();
                    int result = await connection.QueryFirstOrDefaultAsync<int>(idQuery, idParams);
                    return true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(TAG + e);
                return false;
            }
        }   
    }
}
