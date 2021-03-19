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

        public Trading(AlpacaSecret secret, IEnumerable<TradeAccount> tradeAccounts)
        {
          //  Thread.Sleep(180000);
            Console.WriteLine($"started trading");
            Start(secret, tradeAccounts);
        }
        public async void Start(AlpacaSecret secret, IEnumerable<TradeAccount> tradeAccounts)
        {
          //  Thread.Sleep(120000);
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
                    //AssetStatus = AssetStatus.Active,
                    //AssetClass = AssetClass.UsEquity
                }
            );

            // TODO - Print out all assets found on alpaca (this seems like there are lots of stocks missing?)
            /* foreach (var a in assets)
             {
                 Console.WriteLine($"Ticker: {a.Symbol} \t Exchange: {a.Exchange}");
             }*/

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
                    await LongTerm();
                }
            }

            // TODO - EXAMPLE STRATEGY ------- REMOVE THIS WHEN FINISHED ------
           // await Sample();

            //had to put this here because the tradeaccounts were not showing up correctly for me  
            await LongTerm();
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
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
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
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task LongTerm()
        {
            try
            {
                var count = 0;
                //Loops through every asset on alpaca.
                //Not a good or efficient way to do this
                //this could use some more inddicators and it 
                //would be helpful if alpaca would finish the 
                // data api v2 to be able to get more data
                foreach (var items in assets)
                {
                    //this is here because for some reason alpaca
                    //doesn't let you really get to 200 api requests all the time
                    //alpaca would send the error for anything greater than 120 requests a minute
                    //Also seems to get weird when you are running things for more than one account
                    //with one account running at once I could get close to 200 with more than one
                    //it stope a littly above 120 requests even if you use two different programs to
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
                        await SubmitOrder(items.Symbol, 15, price, OrderSide.Buy);
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
                        await SubmitOrder(items.Symbol, 15, price, OrderSide.Sell);
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

        private async Task Day(Preference p)
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

        private async Task Scalp(Preference p)
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

        private async Task Swing(Preference p)
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
        public void Dispose()
        {
            alpacaTradingClient?.Dispose();
            alpacaDataClient?.Dispose();
        }
    }
}