using System;

namespace smart_stock.Models
{
    public class Error
    {
        public string ExceptionMessage { get; set; }
        public string Tag { get; set; }
        public string ApiArea { get; set; }
    }
}