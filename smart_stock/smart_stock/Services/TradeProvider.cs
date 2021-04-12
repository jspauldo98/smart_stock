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

        public async Task<int> RecordTrade(Trade trade)
        {
            Console.WriteLine("In provider for insert");
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
                    int result = await connection.QueryFirstOrDefaultAsync<int>(idQuery, idParams);
                    return result;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(TAG + e);
                return 0;
            }
        }   

        public async Task<TradeAccount> GetTradeAccount(int? tId)
        {
            try
            {
                using (MySqlConnection connection = Connection)
                {
                    string sQuery = "SELECT ID, Title, Description, Amount, Profit, Loss, Net, NumTrades, NumSTrades, NumFTrades, Invested, Cash, DateCreated, DateModified FROM TradeAccount ta WHERE ta.Id = @id";
                    var @param = new {id = tId};
                    connection.Open();
                    TradeAccount ta = await connection.QueryFirstOrDefaultAsync<TradeAccount>(sQuery, @param);
                    // get preference Id    
                    var prefIdQ = "SELECT Preference FROM TradeAccount WHERE Id = @id";
                    var @prefIdParam = new {id = tId};   
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
                    var secQ = "SELECT Id, InformationTechnology, HealthCare, Financials, ConsumerDiscretionary, Communication, Industrials, ConsumerStaples, Energy, Utilities, RealEstate, Materials FROM Sectors WHERE Id = @secId";
                    var @secParam = new {secId = secId};
                    Sector sector = await connection.QueryFirstOrDefaultAsync<Sector>(secQ, @secParam);
                    // get preference
                    var prefQ = "SELECT Id, CapitalToRisk FROM Preference WHERE Id = @pId";
                    var @prefParam = new {pId = prefID};
                    ta.Preference = await connection.QueryFirstOrDefaultAsync<Preference>(prefQ, @prefParam);
                    ta.Preference.RiskLevel = riskLevel;
                    ta.Preference.TradeStrategy = strategy;
                    ta.Preference.Sector = sector;

                    return ta;
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
