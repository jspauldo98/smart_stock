using System;

namespace smart_stock.AlpacaServices
{
    public interface IFirstPaperTrade
    {
        void CommunicateBackgroundWorker(string[] args);
    }
}