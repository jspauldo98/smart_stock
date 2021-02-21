using System;
using System.Runtime.CompilerServices;
using System.Net.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class TradeAccount
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int? Id { get; set; }

        // TODO - Is a portfolio even needed here since its already an object in the service?
        // [Required]
        // public Portfolio Portfolio { get; set; }

        [Required]
        public Preference Preference { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Title { get; set;}

        [Column(TypeName = "decimal(13, 2)")]
        public double Profit { get; set; }

        [Column(TypeName = "decimal(13, 2)")]
        public double Loss { get; set; }

        [Column(TypeName = "decimal(13, 2)")]
        public double Net { get; set;}

        [Column(TypeName = "int(64)")]
        public int? NumTrades { get; set; }

        [Column(TypeName = "int(64)")]
        public int NumSTrades { get; set; }

        [Column(TypeName = "int(64)")]
        public int NumFTrades { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateCreated { get; set;}

        [Column(TypeName = "date")]
        public DateTime DateModified { get; set;}
    }
}