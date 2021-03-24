using System.Net;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alpaca.Markets;
using smart_stock.Models;
using smart_stock.Services;

namespace smart_stock.AlpacaServices
{
    public class Trading : ITrading
    {
        private IAlpacaTradingClient alpacaTradingClient;
        private IAlpacaDataClient alpacaDataClient;
        private IReadOnlyList<IAsset> assets;
        private readonly ITradeProvider _tradeProvider;
        private readonly ILogProvider _logProvider;

        public Trading(AlpacaSecret secret, IEnumerable<TradeAccount> tradeAccounts, ITradeProvider tradeProvider, ILogProvider logProvider) {
            Console.WriteLine($"started trading");
            _tradeProvider = tradeProvider;
            _logProvider = logProvider;
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
                else
                {
                    await Sample(ta.Id);
                } 
            }

            // TODO - EXAMPLE STRATEGY ------- REMOVE THIS WHEN FINISHED ------
            //await Sample();
        }
        // TODO - EXAMPLE STRATEGY ------- REMOVE THIS WHEN FINISHED ------
        private async Task Sample(int? tradeAccountId)
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
                await SubmitOrder("SPY", 1, 0, OrderSide.Buy, tradeAccountId);

                // Wait 20 seconds
                Thread.Sleep(20000);

                // sell example - sell 1 share of SPY at market price
                await SubmitOrder("SPY", 1, 0, OrderSide.Sell, tradeAccountId);
            } catch (WebException ex) when (ex.Response is HttpWebResponse response)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await AwaitRequestRedemption();
                }
            }
        }

        private async Task BlueChip(Preference p, int? tradeAccountId)
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

        private async Task LongTerm(Preference p, int? tradeAccountId)
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

        private async Task Day(Preference p, int? tradeAccountId)
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

        private async Task Scalp(Preference p, int? tradeAccountId)
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

        private async Task Swing(Preference p, int? tradeAccountId)
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
                //Doesn't make sense to calculate the portfolio amount here.
            };
        }
        public void Dispose()
        {
            alpacaTradingClient?.Dispose();
            alpacaDataClient?.Dispose();
        }
    }
}