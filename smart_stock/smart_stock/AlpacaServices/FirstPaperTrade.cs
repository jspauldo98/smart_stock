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
    public class FirstPaperTrade
    {
        private readonly IUserProvider _userProvider;
        private string[] _suppliedArgs;
        //This might need to be static
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private const string Symbol = "SPY";
        private const Decimal scale = 200;
        private IAlpacaTradingClient alpacaTradingClient;
        private IAlpacaDataClient alpacaDataClient;
        private Guid lastTradeId = Guid.NewGuid();

        public FirstPaperTrade(IUserProvider userProvider, string[] suppliedArgs)
        {
            _userProvider = userProvider;
            _suppliedArgs = suppliedArgs;
            CommunicateBackgroundWorker(suppliedArgs);
        }

        public void CommunicateBackgroundWorker(string[] suppliedArgs)
        {
            if (suppliedArgs[0] == "start")
            {
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
                backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
                backgroundWorker.RunWorkerAsync();
            }
            else if (suppliedArgs[0] == "stop")
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object userObject = e.UserState;
            int percentage = e.ProgressPercentage;
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            while(!worker.CancellationPending)
            {
                ExecuteTradingCycle();
                worker.ReportProgress(0, "test");
            }
        }

        private void ExecuteTradingCycle()
        {
            
        }
    }
}