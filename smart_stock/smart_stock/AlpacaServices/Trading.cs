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

namespace smart_stock.AlpacaServices
{
    public class Trading : ITrading
    {
        private IAlpacaTradingClient alpacaTradingClient;
        private IAlpacaDataClient alpacaDataClient;
        private IReadOnlyList<IAsset> assets;
        private readonly IUserProvider _userProvider;
        private readonly ITradeProvider _tradeProvider;
        private readonly ILogProvider _logProvider;
        public Trading(IUserProvider userProvider, ITradeProvider tradeProvider, ILogProvider logProvider) 
        {
            _userProvider = userProvider;
            _tradeProvider = tradeProvider;
            _logProvider = logProvider;
        }

        public async Task GetUserData()
        {
            // First get users alpaca data and trade accounts assigned to a tuple to run in parallel later
            List<(AlpacaSecret, TradeAccount[])> users = await _userProvider.GetUserData();
            // Now execute trading for all users in parallel
            // TODO FOR NOW BREAK AFTER TWO!!!! OTHERWISE PREPARE FOR ANAL ABLITERATION
            Parallel.For(0, users.Count, i => 
            {
                if (i > 1) return;
                Start(users[i].Item1, users[i].Item2);
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
                Thread.Sleep(60000);
                timeUntilClose = closingTime - DateTime.UtcNow;
            }

            Console.WriteLine("Market Nearing close, no more order requests");
            await CancelExistingOrders();
        }

        private async Task<IReadOnlyList<IAsset>> GetTradableTickerList()
        {
            var assets = await alpacaTradingClient.ListAssetsAsync(
                new AssetsRequest
                {
                    AssetStatus = AssetStatus.Active,
                    AssetClass = AssetClass.UsEquity
                }
            );

            return assets;
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
            Console.WriteLine("Alpaca API has exceeded number of requests. Waiting 2 minutes for recovery.");
            await Task.Delay(120000);
        }
        private async Task CheckPreferences(IEnumerable<TradeAccount> tradeAccounts)
        {
            // Loop all trade accounts
            foreach (var ta in tradeAccounts)
            {
                Console.WriteLine(ta.Preference);
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
            try
            {
                // TODO - Run day selling algorithm to see if owned assets need sold.
                // TODO - Run day algorithm to acquire new assets if possible.
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
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
                // TODO - Run swing selling algorithm to see if owned assets need sold.
                // TODO - Run swing algorithm to acquire new assets if possible.
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
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
                return;
            }
            var limitorder = await alpacaTradingClient.PostOrderAsync(
                orderType.Limit(symbol, quantity, price)
            );
            Console.WriteLine($"Submitting {symbol} {orderType} limit order for {quantity} shares at ${price}.");

            Trade trade = new Trade 
            {
                Type = orderType == OrderSide.Buy ? true : false,
                Ticker = symbol,
                //Unsure of what this can be used for
                Amount = 0,
                Price = Decimal.ToDouble(price),
                Quantity = quantity,
                Date = DateTime.Now
            };
            trade.Id = await _tradeProvider.RecordTrade(trade);
            var accountAmount = await alpacaTradingClient.GetAccountAsync();
            Log log = new Log
            {
                TradeAccount = new TradeAccount {Id = tradeAccountId},
                Trade = trade,
                Date = DateTime.Now,
                TradeAccountAmount = Decimal.ToDouble(accountAmount.BuyingPower)
                //Doesn't make sense to calculate the portfolio amount here, can do this later?
            };
            await _logProvider.RecordTradeInLog(log); 
        }

        /* Calculates the RSI of a stock
            @param symbol - Stock ticker to calculate RSI for
            @param timeFrame - Amount and precision of calculations to make
            @param periods - how many periods to use in calculation (standard is 14)
            @param limit - how many data points with RSI are returned
            @returns array of tuples representing DateTime and historical RSI data
            @example - await GetRsi("SPY", TimeFrame.Minute, 14, 100) */
        private async Task<IEnumerable<(DateTime?, decimal)>> GetRsi(
            string symbol, TimeFrame timeFrame, int periods, int limit
        )
        {
            // Init array of tuples to store RSI data
            List<(DateTime?, decimal)> rsiData = new List<(DateTime?, decimal)>();

            // Get market data on symbol given timeframe
            var bars = await GetMarketData(symbol, timeFrame, periods+limit);
            
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
        }

        /* Calculates the SMA of a stock
            @param symbol - Stock ticker to calculate SMA for
            @param timeFrame - Amount and precision of calculations to make
            @param periods - how many periods to use in calculation
            @param limit - how many data points with SMA are returned
            @returns array of tuples representing DateTime and historical SMA data
            @example - await GetSma("SPY", TimeFrame.Day, 180, 500) // this would get the last 500 days of 180SMA data */
        private async Task<IEnumerable<(DateTime?, decimal)>> GetSma(
            string symbol, TimeFrame timeFrame, int periods, int limit
        )
        {
            // Init array of tuples to store SMA data
            List<(DateTime?, decimal)> smaData = new List<(DateTime?, decimal)>();

            // Get market data on symbol given timeframe
            var bars = await GetMarketData(symbol, timeFrame, periods+limit);

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
        }

        /* Retrieves multiple volume points a stock
            @param symbol - Stock ticker to get volumes for
            @param timeFrame - Amount and precision of volume to retrieve
            @param periods - how many data points to use in the retrival 
            @returns array of tuples representing DateTime and historical volume data
            @example - await Getvolume("SPY", TimeFrame.Day, 180) // this would get the last 180 days of volume */
        private async Task<IEnumerable<(DateTime?, decimal)>> GetVolume(
            string symbol, TimeFrame timeFrame, int periods
        )
        {
            // Init array of tuples to store volume data
            List<(DateTime?, decimal)> vData = new List<(DateTime?, decimal)>();

            // Get market data on symbol given timeFrame
            var bars = await GetMarketData(symbol, timeFrame, periods);

            // Build array from market volume collected
            foreach (var b in bars[symbol])
            {
                vData.Add((b.TimeUtc, b.Volume));
            }

            return vData;
        }

        /* Calculates the EMA of a stock (this uses multiplier smoothing of 2)
            @param symbol - Stock ticker to calculate EMA for
            @param timeFrame - Amount and precision of calculations to make
            @param periods - how many periods to use in calculation
            @param limit - how many data points with EMA are returned
            @returns array of tuples representing DateTime and historical EMA data
            @example - await GetEma("SPY", TimeFrame.Day, 180, 500) // this would get the last 500 days of 180EMA data */
        private async Task<IEnumerable<(DateTime?, decimal)>> GetEma(
            string symbol, TimeFrame timeFrame, int periods, int limit
        )
        {
            // Init array of tuples to store EMA data
            List<(DateTime?, decimal)> emaData = new List<(DateTime?, decimal)>();

            // Get market data on symbol given timeFrame
            var bars = await GetMarketData(symbol, timeFrame, periods+limit+1);

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
        }

        /* Calculates the MACD of a stock
            @param symbol - Stock ticker to calculate MACD for
            @param timeFrame - Amount and precision of calculations to make
            @param limit - how many data points with MACD are returned
            @returns array of tuples representing DateTime and historical MACD data where Item2 is the MACD line and Item3 is the signal line
            @example - await GetMacD("SPY", TimeFrame.Day, 100) // this would get the last 100 days MACD data */
        private async Task<IEnumerable<(DateTime?, decimal, decimal)>> GetMacD(
            string symbol, TimeFrame timeFrame, int limit
        )
        {
            // Init array of tuples to store MACD data
            List<(DateTime?, decimal, decimal)> macdData = new List<(DateTime?, decimal, decimal)>();

            // Init EMAs needed
            var lowerEMA = await GetEma(symbol, timeFrame, 12, limit);
            var upperEMA = await GetEma(symbol, timeFrame, 26, limit);

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
        }

        public void Dispose()
        {
            alpacaTradingClient?.Dispose();
            alpacaDataClient?.Dispose();
        }
    }
}