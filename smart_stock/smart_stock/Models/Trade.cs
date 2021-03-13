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

        [Required]
        [Column(TypeName = "bit")]
        public Boolean Type { get; set;}

        [Required]
        [Column(TypeName = "char(4)")]
        public string Ticker { get; set;}

        [Required]
        [Column(TypeName = "decimal(13,2)")]
        public double Amount { get; set;}

        [Required]
        [Column(TypeName = "decimal(13,2)")]
        public double Price { get; set;}

        [Required]
        [Column(TypeName = "decimal(13,2)")]
        public double Quantity { get; set;}

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set;}
    }
}