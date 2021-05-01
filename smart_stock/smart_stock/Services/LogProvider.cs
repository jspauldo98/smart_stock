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

        public async Task<bool> RecordTradeInLog(Log log)
        {
            try 
            {
                using(MySqlConnection connection = Connection)
                {
                    string insertQuery = "INSERT INTO Log (TradeAccount, Trade, Date, TradeAccountAmount, PortfolioAmount) VALUES (@tradeAccountId, @tradeId, @date, @tradeAccountAmount, @portfolio)";
                    var @insertParams = new {tradeAccountId = log.TradeAccount.Id, tradeId = log.Trade.Id, date = log.Date, tradeAccountAmount = log.TradeAccountAmount, portfolio=log.PortfolioAmount};
                    connection.Open();
                    await connection.ExecuteAsync(insertQuery, insertParams);
                    return true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(TAG + e);
                return false;
            }
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

        public async Task<IEnumerable<Log>> GetMinuteLog(int tId)
        {
            string sQuery = "SELECT Id, date_format(Date, '%Y-%m-%d %H:%i') Date, TradeAccountAmount, PortfolioAmount FROM Log WHERE TradeAccount=@id AND Id IN (SELECT MAX(Id) FROM Log GROUP BY date_format(Date, '%Y-%m-%d %H:%i')) ORDER BY Id DESC LIMIT 100;";
            return await GetTimeData(tId, sQuery);
        }

        public async Task<IEnumerable<Log>> GetHourLog(int tId)
        {
            string sQuery = "SELECT Id, date_format(Date, '%Y-%m-%d %H:00') Date, TradeAccountAmount, PortfolioAmount FROM Log WHERE TradeAccount=@id AND Id IN (SELECT MAX(Id) FROM Log GROUP BY date_format(Date, '%Y-%m-%d %H:00')) ORDER BY Id DESC LIMIT 100;";
            return await GetTimeData(tId, sQuery);
        }

        public async Task<IEnumerable<Log>> GetDayLog(int tId)
        {
            string sQuery = "SELECT Id, date_format(Date, '%Y-%m-%d') Date, TradeAccountAmount, PortfolioAmount FROM Log WHERE TradeAccount=@id AND Id IN (SELECT MAX(Id) FROM Log GROUP BY date_format(Date, '%Y-%m-%d')) ORDER BY Id DESC LIMIT 100;";
            return await GetTimeData(tId, sQuery);
        }

        public async Task<IEnumerable<Log>> GetWeekLog(int tId)
        {
            string sQuery = "SELECT Id, date_format(Date, '%Y-%m-%u') Date, TradeAccountAmount, PortfolioAmount FROM Log WHERE TradeAccount=@id AND Id IN (SELECT MAX(Id) FROM Log GROUP BY date_format(Date, '%Y-%m-%u')) ORDER BY Id DESC LIMIT 100;";
            return await GetTimeData(tId, sQuery);
        }

        public async Task<IEnumerable<Log>> GetMonthLog(int tId)
        {
            string sQuery = "SELECT Id, date_format(Date, '%Y-%m') Date, TradeAccountAmount, PortfolioAmount FROM Log WHERE TradeAccount=@id AND Id IN (SELECT MAX(Id) FROM Log GROUP BY date_format(Date, '%Y-%m')) ORDER BY Id DESC LIMIT 100;";
            return await GetTimeData(tId, sQuery);
        }

        public async Task<IEnumerable<Log>> GetYearLog(int tId)
        {
            string sQuery = "SELECT Id, date_format(Date, '%Y-01-01') Date, TradeAccountAmount, PortfolioAmount FROM Log WHERE TradeAccount=@id AND Id IN (SELECT MAX(Id) FROM Log GROUP BY date_format(Date, '%Y-01-01')) ORDER BY Id DESC LIMIT 100;";
            return await GetTimeData(tId, sQuery);
        }

        private async Task<IEnumerable<Log>> GetTimeData(int tId, string query)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    var @param = new {id = tId};
                    connection.Open();
                    var logs = await connection.QueryAsync<Log>(query, @param);
                    logs=logs.Reverse();
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
