using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class Trade
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int? Id { get; set; }

        [Column(TypeName = "char(4)")]
        public string Ticker { get; set; }

        //1 = buy, 0 = sell
        [Column(TypeName = "bit(1)")]
        public bool? Type { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public Decimal? Amount { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public Decimal? Price { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public Decimal? Quantity { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }
    }
}