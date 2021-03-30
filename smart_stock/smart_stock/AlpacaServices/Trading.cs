using System.Net;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alpaca.Markets;
using smart_stock.Models;

namespace smart_stock.AlpacaServices
{
    public class Trading : ITrading
    {
        private IAlpacaTradingClient alpacaTradingClient;
        private IAlpacaDataClient alpacaDataClient;
        private IReadOnlyList<IAsset> assets;

        public Trading(AlpacaSecret secret, IEnumerable<TradeAccount> tradeAccounts){
            Console.WriteLine($"started trading");
            Start(secret, tradeAccounts);
        }
        public async void Start(AlpacaSecret secret, IEnumerable<TradeAccount> tradeAccounts)
        {
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
                new AssetsRequest { 
                    AssetStatus = AssetStatus.Active, AssetClass = AssetClass.UsEquity 
                }
            );

            // TODO - Print out all assets found on alpaca (this seems like there are lots of stocks missing?)
            // foreach (var a in assets)
            // {
            //     Console.WriteLine($"Ticker: {a.Symbol} \t Exchange: {a.Exchange}");
            // }

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
                // Check strategies
                Console.WriteLine($"Checking strategies for: {ta.Title}");
                if (ta.Preference.TradeStrategy.Scalp)
                {
                    await Scalp(ta.Preference);
                }
                if (ta.Preference.TradeStrategy.Day)
                {
                    await Day(ta.Preference);
                }
                if (ta.Preference.TradeStrategy.Swing)
                {
                    await Swing(ta.Preference);
                }
                if (ta.Preference.TradeStrategy.BlueChip)
                {
                    await BlueChip(ta.Preference);
                }
                if (ta.Preference.TradeStrategy.LongTerm)
                {
                    await LongTerm(ta.Preference);
                }
            }

            // TODO - EXAMPLE STRATEGY ------- REMOVE THIS WHEN FINISHED ------
            await Sample();
        }
        // TODO - EXAMPLE STRATEGY ------- REMOVE THIS WHEN FINISHED ------
        private async Task Sample()
        {
            try 
            {
                // Market data example - Get daily price data for SPY over the last 2 trading days.
                var bars = await GetMarketData("SPY", TimeFrame.Day, 2);

                // print data collected
                foreach (var b in bars["SPY"])
                {
                    Console.WriteLine($"$SPY: \n\t time: {b.TimeUtc} \n\t open: {b.Open} \n\t high: {b.High} \n\t low: {b.Low} \n\t close: {b.Close} \n\t volume: {b.Volume}");
                }
                
                // buy example - buy 1 share of SPY at market price
                await SubmitOrder("SPY", 1, 0, OrderSide.Buy);

                // Wait 20 seconds
                Thread.Sleep(20000);

                // sell example - sell 1 share of SPY at market price
                await SubmitOrder("SPY", 1, 0, OrderSide.Sell);
            } catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task BlueChip(Preference p)
        {
            try
            {
                // TODO - Run bluechip selling algorithm to see if owned assets need sold.
                // TODO - Run bluechip algorithm to acquire new assets if possible.
            } catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }            
        }

        private async Task LongTerm(Preference p)
        {
            try
            {
                // TODO - Run longterm selling algorithm to see if owned assets need sold.
                // TODO - Run longterm algorithm to acquire new assets if possible.
            } catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }                 
        }

        private async Task Day(Preference p)
        {
            try
            {
                // TODO - Run day selling algorithm to see if owned assets need sold.
                // TODO - Run day algorithm to acquire new assets if possible.
            } catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }            
        }

        private async Task Scalp(Preference p)
        {
            try
            {
                // TODO - Run scalp selling algorithm to see if owned assets need sold.
                // TODO - Run scalp algorithm to acquire new assets if possible.
            } catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }            
        }

        private async Task Swing(Preference p)
        {
            try
            {
                // TODO - Run swing selling algorithm to see if owned assets need sold.
                // TODO - Run swing algorithm to acquire new assets if possible.
            } catch (WebException ex) when (ex.Response is HttpWebResponse response)
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
        private async Task SubmitOrder(string symbol, Int64 quantity, Decimal price, OrderSide orderType)
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

            // TODO - find a way to log trade and update all users tradeaccount and portfolio information. I don't think we will be able to inject a provider here.
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

        /* Calculates the RSI of a stock
            @param symbol - Stock ticker to calculate SMA for
            @param timeFrame - Amount and precision of calculations to make
            @param periods - how many periods to use in calculation
            @param limit - how many data points with SMA are returned
            @returns array of tuples representing DateTime and historical SMA data
            @example - await GetRsi("SPY", TimeFrame.Day, 180, 500) // this would get the last 500 days of 180SMA data */
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

        public void Dispose()
        {
            alpacaTradingClient?.Dispose();
            alpacaDataClient?.Dispose();
        }
    }
}