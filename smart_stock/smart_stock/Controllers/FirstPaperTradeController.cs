using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using smart_stock.Models;
using smart_stock.Services;
using System.Collections.Generic;
using System.Linq;
using smart_stock.AlpacaServices;

namespace smart_stock.Controllers
{
    [Route("api/first")]
    [ApiController]
    [EnableCors("first")]
    public class FirstPaperTradeController : ControllerBase
    {
        private IFirstPaperTrade _firstPaperTrade;
        public FirstPaperTradeController (IFirstPaperTrade firstPaperTrade)
        {
            _firstPaperTrade = firstPaperTrade;
        }

        [HttpPut]
        public void StopWorkerThread() 
        {
            string[] args = new string[1];
            args[0] = "stop";
            _firstPaperTrade.CommunicateBackgroundWorker(args);
        }

    }
}