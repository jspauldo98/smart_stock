using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class Log
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int? Id { get; set; }

        public TradeAccount TradeAccount { get; set; }

        [Required]
        public Trade Trade { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set;}

        [Required]
        [Column(TypeName = "decimal(13,2)")]
        public double TradeAccountAmount { get; set;}

        [Required]
        [Column(TypeName = "decimal(13,2)")]
        public double PortfolioAmount { get; set;}
    }
}