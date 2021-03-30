using System.Threading.Tasks;
using System.Collections.Generic;
using smart_stock.Models;
using Microsoft.AspNetCore.Mvc;
using smart_stock.Services;
namespace smart_stock.AlpacaServices
{
    public interface ITrading
    {
       Task GetUserData();
        void Dispose();
    }
}