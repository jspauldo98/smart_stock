using Alpaca.Markets;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using smart_stock.Services;


namespace smart_stock.AlpacaServices
{
    public class FirstPaperTrade : IFirstPaperTrade
    {
        private readonly IUserProvider _userProvider;
        //This might need to be static
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private const string Symbol = "SPY";
        private string API_KEY;
        private string API_SECRET;
        private const Decimal scale = 200;
        private IAlpacaTradingClient alpacaTradingClient;
        private IAlpacaDataClient alpacaDataClient;
        private Guid lastTradeId = Guid.NewGuid();

        public FirstPaperTrade(IUserProvider userProvider)
        {
            _userProvider = userProvider;
            Console.WriteLine("Constructor called ");
        }

        public async void CommunicateBackgroundWorker(string[] suppliedArgs)
        {
            if (suppliedArgs[0] == "start")
            {
                var test = await _userProvider.GetUserAlpacaKeys(Int32.Parse(suppliedArgs[1]));
                API_KEY = test.AlpacaKeyId;
                API_SECRET = test.AlpacaKey;
                Console.WriteLine(API_KEY);
                Console.WriteLine(API_SECRET);
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
                backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
                backgroundWorker.RunWorkerAsync();
            }
            else if (suppliedArgs[0] == "stop")
            {
                Console.WriteLine("worker stopped with args");
                backgroundWorker.CancelAsync();
            }
        }

        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object userObject = e.UserState;
            int percentage = e.ProgressPercentage;
        }

        private async void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while(!worker.CancellationPending)
            {
                await ExecuteTradingCycle();
                worker.ReportProgress(0, "test");
            }
        }

        private async Task ExecuteTradingCycle()
        {
            Console.WriteLine("Trade Cycle started");
            alpacaTradingClient = Environments.Paper.GetAlpacaTradingClient(new SecretKey(API_KEY, API_SECRET));

            alpacaDataClient = Environments.Paper.GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));

            // First, cancel any existing orders so they don't impact our buying power.
            var orders = await alpacaTradingClient.ListOrdersAsync(new ListOrdersRequest());
            foreach (var order in orders)
            {
                await alpacaTradingClient.DeleteOrderAsync(order.OrderId);
            }

            // Figure out when the market will close so we can prepare to sell beforehand.
            var calendars = (await alpacaTradingClient
                .ListCalendarAsync(new CalendarRequest().SetTimeInterval(DateTime.Today.GetInclusiveIntervalFromThat())))
                .ToList();
            var calendarDate = calendars.First().TradingDateUtc;
            var closingTime = calendars.First().TradingCloseTimeUtc;

            closingTime = new DateTime(calendarDate.Year, calendarDate.Month, calendarDate.Day, closingTime.Hour, closingTime.Minute, closingTime.Second);

            Console.WriteLine("Waiting for market open...");
            await AwaitMarketOpen();
            Console.WriteLine("Market opened.");

            // Check every minute for price updates.
            TimeSpan timeUntilClose = closingTime - DateTime.UtcNow;
            while (timeUntilClose.TotalMinutes > 15)
            {
                // Cancel old order if it's not already been filled.
                await alpacaTradingClient.DeleteOrderAsync(lastTradeId);

                // Get information about current account value.
                var account = await alpacaTradingClient.GetAccountAsync();
                Decimal buyingPower = account.BuyingPower;
                Decimal portfolioValue = account.Equity;

                // Get information about our existing position.
                var positionQuantity = 0L;
                Decimal positionValue = 0;
                try
                {
                    var currentPosition = await alpacaTradingClient.GetPositionAsync(Symbol);
                    positionQuantity = currentPosition.Quantity;
                    positionValue = currentPosition.MarketValue;
                }
                catch (Exception)
                {
                    // No position exists. This exception can be safely ignored.
                }

                var into = DateTime.Now;
                var from = into.Subtract(TimeSpan.FromMinutes(25));
                var barSet = await alpacaDataClient.ListHistoricalBarsAsync(
                    new HistoricalBarsRequest(Symbol, from, into, BarTimeFrame.Minute).WithPageSize(20));
                var bars = barSet.Items;

                Decimal avg = bars.Average(item => item.Close);
                Decimal currentPrice = bars.Last().Close;
                Decimal diff = avg - currentPrice;

                if (diff <= 0)
                {
                    // Above the 20 minute average - exit any existing long position.
                    if (positionQuantity > 0)
                    {
                        Console.WriteLine("Setting position to zero.");
                        await SubmitOrder(positionQuantity, currentPrice, OrderSide.Sell);
                    }
                    else
                    {
                        Console.WriteLine("No position to exit.");
                    }
                }
                else
                {
                    // Allocate a percent of our portfolio to this position.
                    Decimal portfolioShare = diff / currentPrice * scale;
                    Decimal targetPositionValue = portfolioValue * portfolioShare;
                    Decimal amountToAdd = targetPositionValue - positionValue;

                    if (amountToAdd > 0)
                    {
                        // Buy as many shares as we can without going over amountToAdd.

                        // Make sure we're not trying to buy more than we can.
                        if (amountToAdd > buyingPower)
                        {
                            amountToAdd = buyingPower;
                        }
                        Int32 qtyToBuy = (Int32)(amountToAdd / currentPrice);

                        await SubmitOrder(qtyToBuy, currentPrice, OrderSide.Buy);
                    }
                    else
                    {
                        // Sell as many shares as we can without going under amountToAdd.

                        // Make sure we're not trying to sell more than we have.
                        amountToAdd *= -1;
                        var qtyToSell = (Int64)(amountToAdd / currentPrice);
                        if (qtyToSell > positionQuantity)
                        {
                            qtyToSell = positionQuantity;
                        }

                        await SubmitOrder(qtyToSell, currentPrice, OrderSide.Sell);
                    }
                }

                // Wait another minute.
                Thread.Sleep(60000);
                timeUntilClose = closingTime - DateTime.UtcNow;
            }

            Console.WriteLine("Market nearing close; closing position.");
            await ClosePositionAtMarket();
        }

        public void Dispose()
        {
            alpacaTradingClient?.Dispose();
            alpacaDataClient?.Dispose();
        }

        private async Task AwaitMarketOpen()
        {
            while (!(await alpacaTradingClient.GetClockAsync()).IsOpen)
            {
                await Task.Delay(60000);
            }
        }

        // Submit an order if quantity is not zero.
        private async Task SubmitOrder(Int64 quantity, Decimal price, OrderSide side)
        {
            if (quantity == 0)
            {
                Console.WriteLine("No order necessary.");
                return;
            }
            Console.WriteLine($"Submitting {side} order for {quantity} shares at ${price}.");
            var order = await alpacaTradingClient.PostOrderAsync(
                side.Limit(Symbol, quantity, price));
            lastTradeId = order.OrderId;
        }

        private async Task ClosePositionAtMarket()
        {
            try
            {
                var positionQuantity = (await alpacaTradingClient.GetPositionAsync(Symbol)).Quantity;
                await alpacaTradingClient.PostOrderAsync(
                    OrderSide.Sell.Market(Symbol, positionQuantity));
            }
            catch (Exception)
            {
                // No position to exit.
            }
        }
           
        } 
    }
