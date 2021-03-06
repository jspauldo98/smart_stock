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
    public class PreferenceProvider : IPreferenceProvider
    {
        private readonly IConfiguration _config;
        private readonly string TAG = "TradeStrategy Provider: ";
        public PreferenceProvider(IConfiguration config)
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

        public async Task<IEnumerable<RiskLevel>> GetRiskLevels()
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT Id, Risk, DateAdded FROM RiskLevels";
                    connection.Open();
                    var rls = await connection.QueryAsync<RiskLevel>(sQuery);

                    return rls.ToList();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(TAG + err);
                return null;
            }
        }

        public async Task<string> InsertPreference(Preference preference)
        {   
            try
            {
                int result = -1;
                using (MySqlConnection connection = Connection)
                {                                                
                    // Add entry into TradeStrategies Table  
                    var stratQuery = @"INSERT INTO TradeStrategies (BlueChip, LongTerm, Swing, Scalp, Day) VALUES (@blueChip, @longTerm, @swing, @scalp, @day)";        
                    var @params = new {
                        blueChip = preference.TradeStrategy.BlueChip,
                        longTerm = preference.TradeStrategy.LongTerm,
                        swing    = preference.TradeStrategy.Swing   ,
                        scalp    = preference.TradeStrategy.Scalp   ,
                        day      = preference.TradeStrategy.Day
                    };      
                    connection.Open();
                    result = await connection.ExecuteAsync(stratQuery, @params);  

                    // Retrieve Portfolio Id
                    int portId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM Portfolio ORDER BY id DESC LIMIT 1", null);  

                    // Retrieve Strategy Id
                    int stratId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM TradeStrategies ORDER BY id DESC LIMIT 1", null);  

                    // Add entry to Sector table
                    var secQ = "INSERT INTO Sectors (InformationTechnology, HealthCare, Financials, ConsumerDiscretionary, Communication, Industrials, ConsumerStaples, Energy, Utilities, RealEstate, Materials) VALUES (@infoTech, @healthCare, @fin, @consumeD, @comm, @indust, @consumS, @energy, @util, @realE, @mat)";
                    var @secP = new {
                        infoTech   = preference.Sector.InformationTechnology,
                        healthCare = preference.Sector.HealthCare           ,
                        fin        = preference.Sector.Financials           ,
                        consumeD   = preference.Sector.ConsumerDiscretionary,
                        comm       = preference.Sector.Communication,
                        indust     = preference.Sector.Industrials,
                        consumS    = preference.Sector.ConsumerStaples,
                        energy     = preference.Sector.Energy,
                        util       = preference.Sector.Utilities,
                        realE      = preference.Sector.RealEstate,
                        mat        = preference.Sector.Materials
                    };
                    result = await connection.ExecuteAsync(secQ, @secP);

                    // Retrieve Sector Id
                    int secId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM Sectors ORDER BY id DESC LIMIT 1", null);

                    // Add entry into Preference table
                    var prefQ = "INSERT INTO Preference (RiskLevel, TradeStrategy, Sector, CapitalToRisk) VALUES (@risk, @strat, @sector, @capital)";
                    var @prefP = new {
                        risk    = preference.RiskLevel.Id,
                        strat   = stratId   ,
                        sector  = secId     ,
                        capital = preference.CapitalToRisk
                    };
                    result = await connection.ExecuteAsync(prefQ, @prefP);

                    // Retrieve Preference Id
                    int prefId = await connection.QueryFirstOrDefaultAsync<int>("SELECT id FROM Preference ORDER BY id DESC LIMIT 1", null);  

                    // Add entry to TradeAccount Table                                                       
                    var taQuery = @"INSERT INTO TradeAccount (Portfolio, Preference, Title, Description, Amount, Profit, Loss, Net, NumTrades, NumSTrades, NumFTrades, Invested, Cash, DateCreated) VALUES (@portfolio, @preference, @title, @desc, @amount, @profit, @loss, @net, @numTrades, @numSTrades, @numFTrades, @invested, @cash, @dateCreated)";
                    var @paramsTa = new {
                        portfolio   = portId,
                        preference  = prefId,
                        title       = "Default Account",
                        desc        = "This account was generated upon registration",
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
                    result = -1;
                    result = await connection.ExecuteAsync(taQuery, @paramsTa);                                      
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