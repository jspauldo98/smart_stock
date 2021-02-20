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
    public class TradeStrategyProvider : ITradeStrategiesProvider
    {
        private readonly IConfiguration _config;
        private readonly string TAG = "TradeStrategy Provider: ";
        public TradeStrategyProvider(IConfiguration config)
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
        public async Task<string> InsertTradeStrategy(TradeStrategy tradeStrategy)
        {   
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                
                    var stratQuery = @"INSERT INTO TradeStrategies (BlueChip, LongTerm, Swing, Scalp, Day, DateAdded) VALUES (@blueChip, @longTerm, @swing, @scalp, @day, @dateAdded)";        
                    var @params = new {
                        blueChip = tradeStrategy.BlueChip,
                        longTerm = tradeStrategy.LongTerm,
                        swing = tradeStrategy.Swing,
                        scalp = tradeStrategy.Scalp,
                        day = tradeStrategy.Day,
                        dateAdded = tradeStrategy.DateAdded     
                    };      
                    connection.Open();
                    result = await connection.ExecuteAsync(stratQuery, @params);
                }
                return null;  
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return (TAG + err).ToString();
            }
        }          
    }
}