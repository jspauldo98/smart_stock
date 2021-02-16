using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class TradeStrategy
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int? Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(200)")]
        public string[] Strategy { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateAdded { get; set; }
    }
}