using System.Collections.Immutable;
using System.Xml.Linq;
using System.Diagnostics.SymbolStore;
using System.Runtime;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alpaca.Markets;
using smart_stock.Models;
using smart_stock.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using smart_stock.Common;

namespace smart_stock.AlpacaServices
{
    public class Trading : ITrading
    {
        private IAlpacaTradingClient alpacaTradingClient;
        private IAlpacaDataClient alpacaDataClient;
        private List<IAsset> assets;
        private readonly IUserProvider _userProvider;
        private readonly ITradeProvider _tradeProvider;
        private readonly ILogProvider _logProvider;
        private readonly IPortfolioProvider _portfolioProvider;
        public Trading(IUserProvider userProvider, ITradeProvider tradeProvider, ILogProvider logProvider, IPortfolioProvider portfolioProvider) 
        {
            _userProvider = userProvider;
            _tradeProvider = tradeProvider;
            _logProvider = logProvider;
            _portfolioProvider = portfolioProvider;
        }

        public async Task GetUserData()
        {
            // First get users alpaca data and trade accounts assigned to a tuple to run in parallel later
            List<(AlpacaSecret, TradeAccount[])> users = await _userProvider.GetUserData();
            // Now execute trading for all users in parallel
            // TODO FOR NOW BREAK AFTER TWO!!!! OTHERWISE PREPARE FOR ANAL ABLITERATION
            Parallel.For(0, users.Count, i => 
            {
                int pos = users.Count - 1 - i;
                if (pos < users.Count - 1) return;
                Start(users[pos].Item1, users[pos].Item2);
            });
        }

        public async void Start(AlpacaSecret secret, IEnumerable<TradeAccount> tradeAccounts)
        {
            Console.WriteLine($"started trading");
            // First assign trading client and data client
            alpacaTradingClient = Environments.Paper.GetAlpacaTradingClient(
                new SecretKey(
                    secret.AlpacaKeyId, secret.AlpacaKey
                )
            );
            alpacaDataClient = Environments.Paper.GetAlpacaDataClient(
                new SecretKey(
                    secret.AlpacaKeyId, secret.AlpacaKey
                )
            );

            // Obtain and store all stock tickers tradable on alpaca to supply the strategies with
            assets = await GetTradableTickerList();

            // Now, cancel any existing orders
            await CancelExistingOrders();           

            // Figure out when the market will close so we only send orders during market hours and close orders tha have no filled
            var closingTime = await GetMarketClose();

            Console.WriteLine("Waiting for market open...");
            await AwaitMarketOpen();
            Console.WriteLine("Market opened.");

            // Only conduct strategy up until 5 minutes of market close checking every minute for data
            TimeSpan timeUntilClose = closingTime - DateTime.UtcNow;
            while (timeUntilClose.TotalMinutes > 5)
            {
                await CheckPreferences(tradeAccounts);
                timeUntilClose = closingTime - DateTime.UtcNow;
            }

            Console.WriteLine("Market Nearing close, no more order requests");
            await CancelExistingOrders();

            // TODO Liquidate at the end of market day if day trade???
            // foreach (var ta in tradeAccounts)
            // {
            //     if (ta.Preference.TradeStrategy.Day)
            //     {
            //         Console.WriteLine($"Liquidating {ta.Title}...");
            //         var ownedAssets = await _tradeProvider.RetrieveOwnedAssets(ta.Id);
            //         var alpacaPositions = await alpacaTradingClient.ListPositionsAsync();
            //         foreach (var oAsset in ownedAssets)
            //         {
            //             foreach (var aAsset in alpacaPositions)
            //             {
            //                 if (oAsset.Item2 == aAsset.Symbol)
            //                 {
            //                     var assetPos = await alpacaTradingClient.GetPositionAsync(aAsset.Symbol);
            //                     await SubmitOrder(assetPos.Symbol, assetPos.Quantity, 0, OrderSide.Sell, ta.Id);
            //                 }
            //             }
            //         }
            //     }
            // }
            Dispose();
        }

        private async Task<List<IAsset>> GetTradableTickerList()
        {
            var assets = await alpacaTradingClient.ListAssetsAsync(
                new AssetsRequest
                {
                    AssetStatus = AssetStatus.Active,
                    AssetClass = AssetClass.UsEquity
                }
            );
            List<IAsset> writeableCollection = new List<IAsset>(assets);
            return writeableCollection;
        }
        private async Task CancelExistingOrders()
        {
            var orders = await alpacaTradingClient.ListOrdersAsync(new ListOrdersRequest());
            if (orders.Count > 0) Console.WriteLine("Cancelling Orders...");
            foreach (var order in orders)
            {
                await alpacaTradingClient.DeleteOrderAsync(order.OrderId);
                Console.WriteLine($"{order.Symbol} \t\t for: \\t {order.LimitPrice}");
            }
        }
        private async Task<DateTime> GetMarketClose()
        {
            var calendars = (await alpacaTradingClient
                .ListCalendarAsync(new CalendarRequest().SetTimeInterval(DateTime.Today.GetInclusiveIntervalFromThat())))
                .ToList();
            var calendarDate = calendars.First().TradingDateUtc;
            var closingTime = calendars.First().TradingCloseTimeUtc;

            closingTime = new DateTime(calendarDate.Year, calendarDate.Month, calendarDate.Day, closingTime.Hour, closingTime.Minute, closingTime.Second);
            return closingTime;
        }
        private async Task AwaitMarketOpen()
        {
            while (!(await alpacaTradingClient.GetClockAsync()).IsOpen)
            {
                await Task.Delay(60000);
            }
        }
        private async Task AwaitRequestRedemption()
        {
            Console.WriteLine("Alpaca API has exceeded number of requests. Waiting 60 seconds for recovery.");
            await Task.Delay(60000);
        }
        private async Task CheckPreferences(IEnumerable<TradeAccount> tradeAccounts)
        {
            // Loop all trade accounts
            foreach (var ta in tradeAccounts)
            {
                // Check strategies
                // I need that MF UHHHHH trade account ID for each strategy being used to effectively log
                Console.WriteLine($"Checking strategies for: {ta.Title}");
                if (ta.Preference.TradeStrategy.Scalp)
                {
                    await Scalp(ta.Preference, ta.Id);
                }
                if (ta.Preference.TradeStrategy.Day)
                {
                    await Day(ta.Preference, ta.Id);
                }
                if (ta.Preference.TradeStrategy.Swing)
                {
                    await Swing(ta.Preference, ta.Id);
                }
                if (ta.Preference.TradeStrategy.BlueChip)
                {
                    await BlueChip(ta.Preference, ta.Id);
                }
                if (ta.Preference.TradeStrategy.LongTerm)
                {
                    await LongTerm(ta.Preference, ta.Id);
                }
            }
        }

        private async Task BlueChip(Preference p, int? tradeAccountId)
        {
            try
            {
                // TODO - Run bluechip selling algorithm to see if owned assets need sold.
                // TODO - Run bluechip algorithm to acquire new assets if possible.
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task LongTerm(Preference p, int? tradeAccountId)
        {
            try
            {
                var count = 0;
                //Loops through every asset on alpaca.
                //Not a good or efficient way to do this
                //this could use some more indicators and it 
                //would be helpful if alpaca would finish the 
                // data api v2 to be able to get more data
                foreach (var items in assets)
                {
                    //this is here because for some reason alpaca
                    //doesn't let you really get to 200 api requests all the time
                    //alpaca would send the error for anything greater than 120 requests a minute
                    //Also seems to get weird when you are running things for more than one account
                    //with one account running at once I could get close to 200 with more than one
                    //it stope a little above 120 requests even if you use two different programs to
                    //run each key. I would assume it has something to do with the number of requests
                    //coming from the same computer in one minute
                    if(count > 120)
                    {
                        count = 0;
                        Thread.Sleep(60000);
                    }
                    Console.WriteLine(count);
                    count++;
                    //Get the bars for the last 1000 days of whatever symbol is up
                    //if a company hasn't been listed for 1000 days it will return
                    //how ever many they have been listed for
                    var bars = await GetMarketData(items.Symbol, TimeFrame.Day, 1000);

                    decimal price = 0;

                    decimal[] ema9 = new decimal[1000];
                    decimal[] ema13 = new decimal[1000];
                    decimal[] ema50 = new decimal[1000];
                    decimal emaY9 = 0;
                    decimal emaY13 = 0;
                    decimal emaY50 = 0;
                    var index = 0;

                    //This loop finds the 9, 13, and 50 day ema for whatever symbol is up
                    //I keep them in arrays because I want the last two emas of each
                    //there is definitley a better way to do this I just don't want to fix it yet
                    foreach (var item in bars[items.Symbol])
                    {
                        decimal pNow = item.Close;
                        ema9[index] = (pNow * (2m / 10m) + (emaY9 * (1 - (2m / (10m)))));
                        ema13[index] = (pNow * (2m / 14m) + (emaY13 * (1 - (2m / (14m)))));
                        ema50[index] = (pNow * (2m / 51m) + (emaY50 * (1 - (2m / (51m)))));
                        emaY9 = ema9[index];
                        emaY13 = ema13[index];
                        emaY50 = ema50[index];
                        index++;
                    }

                    //this if statement is just here to make sure we do not go out of the  bounds of an array
                    //could probably be fixed in a different way than this
                    if (bars == null || index == 0 || index == 1)
                    { }
                    //This is checking to see if there is a positive crossover in the emas. So if the 13 day ema is higher than 
                    //the 50 day ema and the 9 day moves above the 13 day than it should buy. Not exactly working right yet but it 
                    //sort of works. Also needs more indicators than just ema most likely.
                    else if ((ema9[index - 1] > ema13[index - 1] && ema9[index - 2] < ema13[index - 2]) && (ema13[index - 1] > ema50[index - 1] && ema13[index - 2] < ema50[index - 2])
                            && (ema9[index - 1] > ema50[index - 1] && ema9[index - 2] < ema50[index - 2]))
                    {
                        //this is here because I was having issues getting the most current price using getLastQuoteAsync
                        //This could be changed because it only gives us the price from 1 minute ago(at least I think it does) could possibly use getLastTradeAsync
                        //could possibly use getLastTradeAsync instead of getBarSetAsync I haven't exactly tried that yet
                        var bars2 = await GetMarketData(items.Symbol, TimeFrame.Minute, 1);
                        foreach (var g in bars[items.Symbol])
                        {
                            price = g.Close;
                        }
                        count++;
                        count++;
                        Console.WriteLine("BUY    " + items.Symbol + "  " + ema9[index - 1] + "  " + ema9[index - 2] + "   " + index + "   " + price + "  " + ema13[index - 1] + "  " + ema13[index - 2] + "  " + ema13[index - 2] + "  " + ema50[index - 1] + "  " + ema50[index - 2]);
                        await SubmitOrder(items.Symbol, 15, price, OrderSide.Buy, tradeAccountId);
                    }
                    //Same as above just the opposite
                    else if ((ema9[index - 1] < ema13[index - 1] && ema9[index - 2] > ema13[index - 2]) && (ema13[index - 1] < ema50[index - 1] && ema13[index - 2] > ema50[index - 2])
                            && (ema9[index - 1] < ema50[index - 1] && ema9[index - 2] > ema50[index - 2]))
                    {
                        var bars2 = await GetMarketData(items.Symbol, TimeFrame.Minute, 1);
                        foreach (var g in bars2[items.Symbol])
                        {
                            price = g.Close;
                        }
                        count++;
                        count++;
                        Console.WriteLine("SELL   " + items.Symbol + "  " + ema9[index - 1] + "  " + ema9[index - 2] + "   " + index + "   " + price + "  " + ema13[index - 1] + "  " + ema13[index - 2] + "  " + ema50[index - 1] + "  " + ema50[index - 2]);
                        await SubmitOrder(items.Symbol, 15, price, OrderSide.Sell, tradeAccountId);
                    }
                }
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task Day(Preference p, int? tradeAccountId)
        {
            bool detailedLogging = false;

            Console.WriteLine($"\t Starting Day trading algorithm... (detailed logging {detailedLogging})");

            //* Selling Algorithm
            await DaySell(p, tradeAccountId, detailedLogging);     

            //* Buying Algorithm
            await DayBuy(p, tradeAccountId, detailedLogging);
        }

        private async Task DaySell(Preference p, int? tradeAccountId, bool detailedLogging)
        {
            const string ALGO_TAG = "*DAY TRADE: SELL ALGO*";

            //* Selling Algorithm 
            // Retrieve owned assets
            // Probably need to make a model for this, but I am being lazy. Item 1 is Id. Item 2, symbol. item3, quantity. item4, price
            var ownedAssets = await _tradeProvider.RetrieveOwnedAssets(tradeAccountId);
            var alpacaPositions = await alpacaTradingClient.ListPositionsAsync();
            // Loop sell algorithm for each owned asset
            foreach(var ownedAsset in ownedAssets)
            {
                if (detailedLogging)
                    Console.WriteLine($"{ALGO_TAG} \t Checking {ownedAsset.Item2}");
                // Check to make sure backend asset maches client position
                bool flag = false;
                foreach(var pos in alpacaPositions)
                {
                    if (pos.Symbol == ownedAsset.Item2 && pos.Quantity == (int)ownedAsset.Item3)
                        flag = true;
                }
                if (!flag)
                {
                    if (detailedLogging)
                        Console.WriteLine($"\t\t\t {ownedAsset.Item2} does not exist in api portfolio. \t Skipping...");
                    continue;
                } 
                try
                {
                    // Get bar 
                    var barsell = await GetMarketData(ownedAsset.Item2, TimeFrame.Minute, 1);
                    var aPos = await alpacaTradingClient.GetPositionAsync(ownedAsset.Item2);                    

                    // Check profitability, use stop loss on varying risk levels
                    decimal profit = aPos.UnrealizedProfitLossPercent *100;
                    if (detailedLogging)
                        Console.WriteLine($"{ALGO_TAG} \t {ownedAsset.Item2} \t profitability: {profit}");
                    switch(p.RiskLevel.Risk)
                    {
                        case "Low":
                            if (profit < -1.0m || profit > 0.1m)
                            {
                                await SubmitOrder(ownedAsset.Item2, (long)ownedAsset.Item3, aPos.AssetCurrentPrice-.01m,  OrderSide.Sell, tradeAccountId);
                                continue;
                            }
                        break;
                        case "Moderate":
                            if (profit < -3.0m || profit > 0.2m)
                            {
                                await SubmitOrder(ownedAsset.Item2, (long)ownedAsset.Item3, aPos.AssetCurrentPrice-.01m,  OrderSide.Sell, tradeAccountId);
                                continue;
                            }
                        break;
                        case "High":
                            if (profit < -3.0m || profit > 5m)
                            {
                                await SubmitOrder(ownedAsset.Item2, (long)ownedAsset.Item3, aPos.AssetCurrentPrice-.01m,  OrderSide.Sell, tradeAccountId);
                                continue;
                            }
                        break;
                        case "Aggressive":
                            if (profit < -5.0m || profit > 1m)
                            {
                                await SubmitOrder(ownedAsset.Item2, (long)ownedAsset.Item3, aPos.AssetCurrentPrice-.01m,  OrderSide.Sell, tradeAccountId);
                                continue;
                            }                                  
                        break;
                    }
                    
                } catch (Alpaca.Markets.RestClientErrorException)
                {
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task DayBuy(Preference p, int? tradeAccountId, bool detailedLogging)
        {
            const string ALGO_TAG = "*DAY TRADE: BUY ALGO*";
            
            //* Buying Algorithm 
            // Slim and randomize asset list to not include already owned assets
            var tradeableSymbols = await GetTradableTickerList();
            Dictionary<string, decimal?> dbAssets = new Dictionary<string, decimal?>();
            var ownedAssets = await _tradeProvider.RetrieveOwnedAssets(tradeAccountId);
            foreach(var asset in ownedAssets)
                dbAssets.Add(asset.Item2, asset.Item3);
            var deltaSize = tradeableSymbols.Count - 100;
            // Something has gone wrong if deltaSize is 0 or less
            if (deltaSize < 1) return;
            tradeableSymbols.ShuffleList<IAsset>();
            tradeableSymbols.RemoveRange(100, deltaSize);
            for (int s = 0; s < tradeableSymbols.Count - 1; s++)
            {
                if (dbAssets.ContainsKey(tradeableSymbols[s].Symbol))
                {
                    if (detailedLogging)
                        Console.WriteLine($"{ALGO_TAG} \t We already have " + tradeableSymbols[s].Symbol + " in our owned assets, removing now");
                    tradeableSymbols.RemoveAt(s);
                }
            }
            if (detailedLogging)
                Console.WriteLine($"{ALGO_TAG} \t Starting Day Buy Algorithm...");
            // First, scan for assets with possible setups
            foreach(var asset in tradeableSymbols)
            {
                try
                {
                    // Average daily trading volume must be greater that 1M for a 30 day lookback
                    var vol = await GetVolume(asset.Symbol, TimeFrame.Day, 30);
                    if (vol == null) continue;
                    decimal vAvg = 0;
                    foreach(var v in vol)
                        vAvg += v.Item2;
                    vAvg /= vol.Count;                    
                    if (vAvg < 1000000)
                        continue;
                    if (detailedLogging)
                        Console.WriteLine($"{ALGO_TAG} \t {asset.Symbol} \t Accepting 30D avg vol: {vAvg}");

                    // 180 SMA must show a positive uptrend for 180 period with 30hr lookback (long term uptrend)
                    var sma180 = await GetSma(asset.Symbol, TimeFrame.FiveMinutes, 180, 365);
                    if (sma180 == null)
                    {
                        if (detailedLogging)
                            Console.WriteLine($"\t\t\t\t\t\t Rejecting {asset.Symbol}...");
                        continue;
                    }
                    var sma180Trend = sma180[sma180.Count-1].Item2 - sma180[0].Item2;                    
                    if (sma180Trend < 0)
                    {
                        if (detailedLogging)
                            Console.WriteLine($"\t\t\t\t\t\t Rejecting {asset.Symbol}...");
                        continue;
                    }
                    if (detailedLogging)
                        Console.WriteLine($"{ALGO_TAG} \t {asset.Symbol} \t Accepting 180 SMA Trend: {sma180Trend}");

                    // 20 sma must show a positive uptrend for a 60 min lookback (short term)
                    var sma20 = await GetSma(asset.Symbol, TimeFrame.Minute, 20, 60);
                    if (sma20 == null)
                    {
                        if (detailedLogging)
                            Console.WriteLine($"\t\t\t\t\t\t Rejecting {asset.Symbol}...");
                        continue;
                    }
                    var sma20Trend = sma20[sma20.Count-1].Item2 - sma20[0].Item2;                    
                    if (sma20Trend < 0)
                    {
                        if (detailedLogging)
                            Console.WriteLine($"\t\t\t\t\t\t Rejecting {asset.Symbol}...");
                        continue;
                    }
                    if (detailedLogging)
                        Console.WriteLine($"{ALGO_TAG} \t {asset.Symbol} \t Accepting 20 SMA Trend: {sma20Trend}");

                    // RSI must not be > 75 for 5 minute timeframe with a ~100 min lookback
                    var rsi5 = await GetRsi(asset.Symbol, TimeFrame.Minute, 14, 60);
                    if (rsi5 == null)
                    {
                        if (detailedLogging)
                            Console.WriteLine($"\t\t\t\t\t\t Rejecting {asset.Symbol}...");
                        continue;
                    }
                    var currentRsi = rsi5.LastOrDefault().Item2;
                    if (currentRsi > 75)
                    {
                        if (detailedLogging)
                            Console.WriteLine($"\t\t\t\t\t\t Rejecting {asset.Symbol}...");
                        continue;
                    }
                    if (detailedLogging)
                        Console.WriteLine($"{ALGO_TAG} \t {asset.Symbol} \t Accepting RSI Val: {currentRsi}");

                    // RSI must be uptrending 
                    var rsiTrend = rsi5[rsi5.Count-1].Item2 - rsi5[0].Item2;                    
                    if (rsiTrend < 0)
                    {
                        if (detailedLogging)
                            Console.WriteLine($"\t\t\t\t\t\t Rejecting {asset.Symbol}...");
                        continue;
                    }
                    if (detailedLogging)
                        Console.WriteLine($"{ALGO_TAG} \t {asset.Symbol} \t Accepting RSI Trend: {rsiTrend}");

                    // Retrieve trade account info and buy
                    var ta = await _tradeProvider.GetTradeAccount(tradeAccountId);
                    decimal amount = (decimal)ta.Amount * (decimal)(p.CapitalToRisk/100);
                    var bar = await GetMarketData(asset.Symbol, TimeFrame.Minute, 1);
                    decimal price = bar[asset.Symbol].LastOrDefault().Close+.02m;
                    decimal quantity = amount / price;

                    // Make sure trade account has available cash 
                    if (ta.Cash-(double)(quantity*price) < 0)
                    {
                        Console.WriteLine($"{ALGO_TAG} \t {ta.Title} \t does not have sufficient cash for purchasing...");
                        continue;
                    }  
                    
                    await SubmitOrder(asset.Symbol, (long)quantity, price, OrderSide.Buy, tradeAccountId);

                } catch (Alpaca.Markets.RestClientErrorException)
                {
                    await AwaitRequestRedemption();
                }
            }
            
        }

        private async Task Scalp(Preference p, int? tradeAccountId)
        {
            try
            {
                // TODO - Run scalp selling algorithm to see if owned assets need sold.
                // TODO - Run scalp algorithm to acquire new assets if possible.
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task Swing(Preference p, int? tradeAccountId)
        {
            try
            {
                /*Begin by determing whether we have a swing high or swing low on a given ticker
                  if swing high, consider short trade (Buy high, sell when low).
                  if swing low, consider long trade (Buy low, sell high).
                  Use 5-50 term EMAS for short term preference, favor short term crossover. for 
                  example, get current EMA, then, get 20 day EMA. If current EMA is the same as or higher than
                  20 day, we have a bullish high swing. If current EMA is slightly lower or a lot lower than 20 day
                  we have a bearish swing.
                  Get the volume for the past 20 days, and then the volume for the last three hours. If greater than 20
                  day volume, bullish swing. If volume less than 20 day, bearish swing.
                  Finally, check 14 period RSI. If over 70, ticker is overbought, and should open a bearish
                  short position. If beneath 30, the ticker is oversold, and should open a long bullish position

                */
                bool detailedLogging = true;
                await SwingBuy(p, tradeAccountId, detailedLogging);
                await SwingSell(p, tradeAccountId, detailedLogging);
                //Swing sell is called periodically in swing buy
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task SwingBuy(Preference preference, int? tradeAccountId, bool detailedLogging)
        {
            //Experiment with market orders here, as we want orders to get filled faster with swings, regardless if asset
            //tracking has a stroke, may turn that off for now.
            //We really only want like, 200 symbols, compute the difference between actual size and desired size.
            
            //We will first need to compile lists of owned assets in our DB, before running 
            //through a list of tradeable tickers. Make a dictionary with ticker key, and quantity values for each.
            //If we have an symbol already in our account, throw out the current ticker about to be traded, and go for another.

            var tradeableSymbols = await GetTradableTickerList();
            Dictionary<string, decimal?> dbAssets = new Dictionary<string, decimal?>();

            var ownedAssets = await _tradeProvider.RetrieveOwnedAssets(tradeAccountId);

            foreach(var asset in ownedAssets)
            {
                dbAssets.Add(asset.Item2, asset.Item3);
            }
            var deltaSize = tradeableSymbols.Count - 75;
            if (deltaSize < 0)
            {
                //If the list is smaller than the target size, something is wrong. This will never be the
                //case on a normal trading day, and we need to shut the function down.
                return;
            }
            else 
            {
                //cut list down to 200 symbols, and randomize, then compare with current items in our owned
                //asset dictionary. Throw out values that are alread in DB.
                tradeableSymbols.ShuffleList<IAsset>();
                tradeableSymbols.RemoveRange(75, deltaSize);
                for (int s = 0; s < tradeableSymbols.Count - 1; s++)
                {
                    if (dbAssets.ContainsKey(tradeableSymbols[s].Symbol))
                    {
                        if (detailedLogging)
                        {
                            Console.WriteLine("We already have " + tradeableSymbols[s].Symbol + " in our owned assets, removing now");
                        }
                        tradeableSymbols.RemoveAt(s);
                    }
                }
            }

            //Create a new timer with an event handler that will automatically handle a timed event
            //for selling current assets. Event is called every hour. 
            //TODO Still need to check for market close conditions.
            
            foreach(var ticker in tradeableSymbols)
            {
                // try
                // {
                    
                    var vol20Day = await GetVolume(ticker.Symbol, TimeFrame.Day, 20);
                    var vol3Hour = await GetVolume(ticker.Symbol, TimeFrame.Day, 1);
                    if(vol3Hour == null || vol20Day == null)
                    {
                        Console.WriteLine("Volume is null");
                        continue;
                    }
                    //Get average volume for 20 day and 3 hour lookbacks. If three hour volume is greater than 
                    //twenty day volume plus a factor of 200,000, this will signal a bullish swing
                    double avg20Vol = 0;
                    double avg3Vol = 0;
                    for (int y = 0; y < vol20Day.Count; y++)
                    {
                        avg20Vol += (double) vol20Day[y].Item2;
                    }
                    for (int z = 0; z < vol3Hour.Count; z++)
                    {
                        avg3Vol += (double) vol3Hour[z].Item2;
                    }
                    avg20Vol /= vol20Day.Count;
                    avg3Vol /= vol3Hour.Count;
                    //TODO, set up a catch for bearish volume.
                    if (avg3Vol < avg20Vol + 60000)
                    {
                        Console.WriteLine("Volume does not indicate a bullish swing for " + ticker.Symbol);
                        continue;
                    }
                    Console.WriteLine("Volume for " + ticker.Symbol + " has been determined to be in an acceptable bullish range.");
                    var rsi20Day = await GetRsi(ticker.Symbol, TimeFrame.Day, 70, 20);
                    if (rsi20Day == null)
                    {
                        Console.WriteLine("RSI for " + ticker.Symbol + "is null");
                        continue;
                    }
                    if (rsi20Day.LastOrDefault().Item2 > 70)
                    {
                        //overbought, open a short position
                        //placeShortOrder(...)
                        continue;
                    }
                    if (rsi20Day.LastOrDefault().Item2 <= 37)
                    {
                        //oversold, open a bullish position
                        Console.WriteLine("20 day RSI is within oversold conditions. All conditions have been met, executing order for " + ticker.Symbol + " now.");
                        var ta = await _tradeProvider.GetTradeAccount(tradeAccountId);
                        decimal amount = (decimal)ta.Cash * (decimal)(preference.CapitalToRisk/100);
                        var bar = await GetMarketData(ticker.Symbol, TimeFrame.Minute, 1);
                        decimal price = bar[ticker.Symbol].LastOrDefault().Close+.02m;
                        decimal quantity = amount/price;
                        
                        if (ta.Cash-(double)(quantity*price) < 0)
                        {
                            Console.WriteLine("Unable to buy " + ticker.Symbol + " in Swing Buy, insufficient funds.");
                            continue;
                        }
                        await SubmitOrder(ticker.Symbol, (long)quantity, 0, OrderSide.Buy, tradeAccountId);
                    }
                    Console.WriteLine("RSI conditions not met for " + ticker.Symbol);
                }
                catch (Alpaca.Markets.RestClientErrorException e)
                {
                    Console.WriteLine(e.ToString());
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task SwingSell(Preference preference, int? tradeAccountId, bool detailedLogging)
        {
            Console.WriteLine("selling called for swing");
            var ownedAssets = await _tradeProvider.RetrieveOwnedAssets(tradeAccountId);
            var alpacaAssets = await alpacaTradingClient.ListPositionsAsync();

            Dictionary<string, decimal?> dbAssets = new Dictionary<string, decimal?>();

            foreach(var asset in ownedAssets)
            {
                dbAssets.Add(asset.Item2, asset.Item3);
            }
            
            //Do we really need this? All stored assets should be in our DB, and if alpaca doesn't
            //have it, catch the exception and move on for now.
            /*for (int s = 0; s < alpacaAssets.Count; s++)
            {
                if (!dbAssets.ContainsKey(alpacaAssets[s].Symbol))
                {
                    if (detailedLogging)
                    {
                        Console.WriteLine(alpacaAssets[s].Symbol + " does not exist in user portfolio, removing now");
                    }
                }
            }*/

            foreach(var asset in dbAssets)
            {
                Console.WriteLine(asset.Key);
                try
                {
                    var bar = await GetMarketData(asset.Key, TimeFrame.Minute, 1);
                    var pos = await alpacaTradingClient.GetPositionAsync(asset.Key);

                    decimal profit = pos.UnrealizedProfitLossPercent*100;
                    Console.WriteLine("Profitability for " + asset.Key + " is " + profit.ToString());
                    if (asset.Key == "NAKD")
                    {
                        //I'm calling the shots with this one.
                        continue;
                    }

                    switch(preference.RiskLevel.Risk)
                    {
                        case "Low":
                            if (profit < -2.0m || profit > 0.5m)
                            {
                                await SubmitOrder(asset.Key, (long)asset.Value, 0,  OrderSide.Sell, tradeAccountId);
                                continue;
                            }
                        break;
                        case "Moderate":
                            if (profit < -3.0m || profit > 0.75m)
                            {
                                await SubmitOrder(asset.Key, (long)asset.Value, 0,  OrderSide.Sell, tradeAccountId);
                                continue;
                            }
                        break;
                        case "High":
                            if (profit < -3.0m || profit > 1.5m)
                            {
                                await SubmitOrder(asset.Key, (long)asset.Value, 0,  OrderSide.Sell, tradeAccountId);
                                continue;
                            }
                        break;
                        case "Aggressive":
                            if (profit < -5.0m || profit > 5.0m)
                            {
                                await SubmitOrder(asset.Key, (long)asset.Value, 0,  OrderSide.Sell, tradeAccountId);
                                continue;
                            }                                  
                        break;
                    }
                }
                catch (Alpaca.Markets.RestClientErrorException e)
                {
                    Console.WriteLine(e.ToString());
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task<IReadOnlyDictionary<string, IReadOnlyList<IAgg>>> GetMarketData(
            string symbol, TimeFrame timeFrame, int limit
        )
        {
            var bars = await alpacaDataClient.GetBarSetAsync(
                new BarSetRequest(symbol, timeFrame) { Limit = limit }
            );
            return bars;
        }
        private async Task SubmitOrder(string symbol, Int64 quantity, Decimal price, OrderSide orderType, int? tradeAccountId)
        {
            if (quantity == 0)
            {
                Console.WriteLine("No order necessary");
                return;
            }
            if (price == 0)
            {
                var order = await alpacaTradingClient.PostOrderAsync(
                    orderType.Market(symbol, quantity)
                );
                Console.WriteLine($"Submitting {symbol} {orderType} market order for {quantity} shares at market value."); 
                // wait for the order to be filled before logging
                if (await checkOrderStatus(order))
                {
                    var log = await logOrder(tradeAccountId, await alpacaTradingClient.GetOrderAsync(order.OrderId));
                    await _logProvider.RecordTradeInLog(log);
                }  
                return;
            }
            var limitorder = await alpacaTradingClient.PostOrderAsync(
                orderType.Limit(symbol, quantity, price)
            );
            Console.WriteLine($"Submitting {symbol} {orderType} limit order for {quantity} shares at ${price}.");
            // wait for the order to be filled before logging
            if (await checkOrderStatus(limitorder))
            {
                var limitlog = await logOrder(tradeAccountId, await alpacaTradingClient.GetOrderAsync(limitorder.OrderId));
                await _logProvider.RecordTradeInLog(limitlog);
            } 
        }

        /* 
            Checks every second for an order to be filled. Returns true if filled. returns false if no
            fill after a minute and cancels the order.
        */
        private async Task<bool> checkOrderStatus(IOrder order)
        {
            Console.WriteLine("Waiting for order to be filled...");
            int i = 0;
            var updateorder = await alpacaTradingClient.GetOrderAsync(order.OrderId);
            while (updateorder.OrderStatus != OrderStatus.Filled)
            {
                if (i==20)
                {
                    await CancelExistingOrders();
                    return false;
                }
                Thread.Sleep(1000);
                i++;
                updateorder = await alpacaTradingClient.GetOrderAsync(order.OrderId);
            }
            Console.WriteLine("Order filled");
            return true;
        }
        private async Task<smart_stock.Models.Log> logOrder(int? tradeAccountId, IOrder order)
        {
            TradeAccount ta = await _tradeProvider.GetTradeAccount(tradeAccountId);
            var side = order.OrderSide;

            // Update cash/invested for trade account and portfolio
            if (side == OrderSide.Buy)
            {
                ta.Cash -= (double)order.AverageFillPrice * (double)order.Quantity;
                ta.Invested += (double)order.AverageFillPrice * (double)order.Quantity;
                ta.Portfolio.Invested += (double)order.AverageFillPrice * (double)order.Quantity;
                Console.WriteLine("Portfolio cash is " + ta.Portfolio.Cash + " with order fill price " + order.AverageFillPrice);
            }
            else
            {
                var owned = await _tradeProvider.RetrieveOwnedAsset(ta.Id, order.Symbol);

                ta.Cash += (double)order.AverageFillPrice * (double)order.Quantity;
                ta.Invested -= (double)owned.Item1 * (double)owned.Item2;
                ta.Portfolio.Invested -= (double)owned.Item1 * (double)owned.Item2;

                // Calculate profit/losses and successful/failed trades for trade account
                double difference = (double)((order.Quantity * order.AverageFillPrice)-(owned.Item1 * owned.Item2));
                if (difference > 0)
                {                    
                    ta.NumSTrades++;
                }
                else
                {
                    ta.NumFTrades++;
                }
                ta.Profit += difference;
                ta.Amount += difference;
                ta.Portfolio.Profit += difference;
                ta.Portfolio.Amount += difference;
                
                // update net values
                ta.Net = ta.Profit - ta.Loss;
                ta.Portfolio.Net = ta.Portfolio.Profit - ta.Portfolio.Loss;

                // Increment number of trades for trade account
                ta.NumTrades++;
            }

            // Update db with account and portfolio updates
            await _portfolioProvider.UpdatePortfolio(ta.Portfolio, ta.Portfolio.Id);
            await _portfolioProvider.UpdateTradeAccount(ta, (int)ta.Id);

            // Create a new Trade Object that resembles the trade            
            Trade trade = new Trade 
            {                
                Type = side == OrderSide.Buy ? true : false,
                Ticker = order.Symbol,
                Amount = order.Quantity*order.AverageFillPrice,
                Price = order.AverageFillPrice,
                Quantity = order.Quantity,
                Date = DateTime.Now
            };

            trade.Id = await _tradeProvider.RecordTrade(trade, ta);

            // Create a new Log Object that resembles outside trade details
            smart_stock.Models.Log log = new smart_stock.Models.Log
            {          
                TradeAccount = ta,
                Trade = trade,      
                Date = DateTime.Now,
                TradeAccountAmount = ta.Amount,
                PortfolioAmount = ta.Portfolio.Amount
            };
            return log;
        }

        /* Calculates the RSI of a stock
            @param symbol - Stock ticker to calculate RSI for
            @param timeFrame - Amount and precision of calculations to make
            @param periods - how many periods to use in calculation (standard is 14)
            @param limit - how many data points with RSI are returned
            @returns array of tuples representing DateTime and historical RSI data
            @example - await GetRsi("SPY", TimeFrame.Minute, 14, 100) */
        private async Task<List<(DateTime?, decimal)>> GetRsi(
            string symbol, TimeFrame timeFrame, int periods, int limit
        )
        {
            try
            {
                // Init array of tuples to store RSI data
                List<(DateTime?, decimal)> rsiData = new List<(DateTime?, decimal)>();

                // Get market data on symbol given timeframe
                var bars = await GetMarketData(symbol, timeFrame, periods+limit);

                if (bars[symbol].Count < periods+limit-1) return null;
                
                // Calculate gain and loss for first period bars
                decimal gainAvg = 0, lossAvg= 0;
                foreach (var b in bars[symbol].Take(periods+1))
                {
                    if (b.Close - b.Open > 0) 
                        gainAvg += (b.Close - b.Open);
                    else if (b.Open - b.Close > 0)
                        lossAvg += (b.Open - b.Close);
                }
                gainAvg /= periods;
                lossAvg /= periods;

                // Calculate the first RS and RSI
                decimal currRS = gainAvg/lossAvg;
                decimal currRSI = 0;
                if (currRS == 0) currRSI = 100;
                else if (currRS > 0) currRSI = 100-(100/(1+currRS));
                else currRSI = 0;

                // Add to array
                rsiData.Add((bars[symbol][periods].TimeUtc, currRSI));

                // Loop bars for symbol to get rsi for each point. Skip period bars
                foreach (var b in bars[symbol].Skip(periods+1))
                {
                    // Calculate gain/loss
                    decimal gain = 0, loss = 0;
                    if (b.Close - b.Open > 0) 
                        gain = (b.Close - b.Open);
                    else if (b.Open - b.Close > 0)
                        loss = (b.Open - b.Close);
                    gainAvg = ((gainAvg*(periods-1))+gain)/periods;
                    lossAvg = ((lossAvg*(periods-1))+loss)/periods;

                    // Calculate RS
                    var rs = gainAvg / lossAvg;                

                    // Calculate RSI
                    decimal rsi = 0;
                    if (rs == 0) rsi = 100;
                    else if (rs > 0) rsi = 100-(100/(1+rs));
                    else rsi = 0;

                    // Add to array 
                    rsiData.Add((b.TimeUtc, rsi));
                }

                return rsiData;
            } catch
            {
                return null;
            }
        }

        /* Calculates the SMA of a stock
            @param symbol - Stock ticker to calculate SMA for
            @param timeFrame - Amount and precision of calculations to make
            @param periods - how many periods to use in calculation
            @param limit - how many data points with SMA are returned
            @returns array of tuples representing DateTime and historical SMA data
            @example - await GetSma("SPY", TimeFrame.Day, 180, 500) // this would get the last 500 days of 180SMA data */
        private async Task<List<(DateTime?, decimal)>> GetSma(
            string symbol, TimeFrame timeFrame, int periods, int limit
        )
        {
            try
            {
                // Init array of tuples to store SMA data
                List<(DateTime?, decimal)> smaData = new List<(DateTime?, decimal)>();

                // Get market data on symbol given timeframe
                var bars = await GetMarketData(symbol, timeFrame, periods+limit);

                if (bars[symbol].Count < periods+limit-1) return null;

                // Calculate SMA data
                for (int i = 0; i < limit; i++)
                {
                    decimal sma = 0;
                    foreach (var b in bars[symbol].Skip(i).Take(periods+1))
                        sma += b.Close;
                    sma /= periods;
                    smaData.Add((bars[symbol][periods+i].TimeUtc, sma));
                }

                return smaData;
            } catch
            {
                return null;
            }
        }

        /* Retrieves multiple volume points a stock
            @param symbol - Stock ticker to get volumes for
            @param timeFrame - Amount and precision of volume to retrieve
            @param periods - how many data points to use in the retrival 
            @returns array of tuples representing DateTime and historical volume data
            @example - await Getvolume("SPY", TimeFrame.Day, 180) // this would get the last 180 days of volume */
        private async Task<List<(DateTime?, decimal)>> GetVolume(
            string symbol, TimeFrame timeFrame, int periods
        )
        {
            try
            {
                // Init array of tuples to store volume data
                List<(DateTime?, decimal)> vData = new List<(DateTime?, decimal)>();

                // Get market data on symbol given timeFrame
                var bars = await GetMarketData(symbol, timeFrame, periods);
                if (bars[symbol].Count < periods-1) return null;

                // Build array from market volume collected
                foreach (var b in bars[symbol])
                {
                    vData.Add((b.TimeUtc, b.Volume));
                }

                return vData;
            } catch 
            {
                return null;
            }
        }

        /* Calculates the EMA of a stock (this uses multiplier smoothing of 2)
            @param symbol - Stock ticker to calculate EMA for
            @param timeFrame - Amount and precision of calculations to make
            @param periods - how many periods to use in calculation
            @param limit - how many data points with EMA are returned
            @returns array of tuples representing DateTime and historical EMA data
            @example - await GetEma("SPY", TimeFrame.Day, 180, 500) // this would get the last 500 days of 180EMA data */
        private async Task<List<(DateTime?, decimal)>> GetEma(
            string symbol, TimeFrame timeFrame, int periods, int limit
        )
        {
            try
            {
                // Init array of tuples to store EMA data
                List<(DateTime?, decimal)> emaData = new List<(DateTime?, decimal)>();

                // Get market data on symbol given timeFrame
                var bars = await GetMarketData(symbol, timeFrame, periods+limit+1);

                if (bars[symbol].Count < periods+limit) return null;

                // Calculate EMA data for the first period bars
                // Note this first ema is for peiods +1 bar not period bar
                decimal ema = 0;
                foreach (var b in bars[symbol].Take(periods))
                    ema += b.Close;
                ema /= periods;
                
                // Add to array (again note that this is for periods +1 bar)
                emaData.Add((bars[symbol][periods+2].TimeUtc, ema));

                // Loop bars for symbol to get ema for the rest of the points
                int index = periods +1;
                foreach (var b in bars[symbol].Skip(periods).Take(limit-1))
                {
                    ema = ((b.Close - ema)*(2m/(periods+1))) + ema;
                    if (index != periods+1)
                        emaData.Add((bars[symbol][index+1].TimeUtc, ema));
                    index++;
                }
                return emaData;
            } catch 
            {
                return null;
            }
        }

        /* Calculates the MACD of a stock
            @param symbol - Stock ticker to calculate MACD for
            @param timeFrame - Amount and precision of calculations to make
            @param limit - how many data points with MACD are returned
            @returns array of tuples representing DateTime and historical MACD data where Item2 is the MACD line and Item3 is the signal line
            @example - await GetMacD("SPY", TimeFrame.Day, 100) // this would get the last 100 days MACD data */
        private async Task<List<(DateTime?, decimal, decimal)>> GetMacD(
            string symbol, TimeFrame timeFrame, int limit
        )
        {
            try
            {
                // Init array of tuples to store MACD data
                List<(DateTime?, decimal, decimal)> macdData = new List<(DateTime?, decimal, decimal)>();

                // Init EMAs needed
                var lowerEMA = await GetEma(symbol, timeFrame, 12, limit);
                var upperEMA = await GetEma(symbol, timeFrame, 26, limit);

                if (lowerEMA == null || upperEMA == null) return null;

                // Combine lower and upper EMAs to more easily loop through later
                var emas = lowerEMA.Zip(upperEMA, (l, h) => new {
                    Lower = l, 
                    Upper = h
                });

                // Subtract to get MACD line
                List<(DateTime?, decimal)> macd =  new List<(DateTime?, decimal)>();
                foreach (var lnh in emas)
                {
                    macd.Add((lnh.Lower.Item1, lnh.Upper.Item2 - lnh.Lower.Item2));
                }

                // Calculate signal line from MACD line, want 9EMA
                // Calculate ema data dor the first data point
                decimal ema = 0;
                foreach (var s in macd.Take(9))
                    ema += s.Item2;
                ema /= 9;
                // Add First to data
                macdData.Add((macd[11].Item1, macd[11].Item2, ema));
                // Loop the rest to get rest of ema
                int index = 10;
                foreach (var s in macd.Skip(9).Take(limit-1))
                {
                    ema = ((s.Item2 - ema)*(2m/(10))) + ema;
                    if (index != 10)
                        macdData.Add((s.Item1, s.Item2, ema));
                    index++;
                }

                return macdData;
            } catch 
            {
                return null;
            }
        }

        public void Dispose()
        {
            alpacaTradingClient?.Dispose();
            alpacaDataClient?.Dispose();
        }

        public async Task ElapsedSellMethod(object sender, System.Timers.ElapsedEventArgs e, Preference preference, int? tradeAccountId, bool detailedLogging)
        {
            await SwingSell(preference, tradeAccountId, detailedLogging);
            return;
        }
    }
}