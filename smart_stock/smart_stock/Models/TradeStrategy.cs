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
        [Column(TypeName = "bit")]
        public bool BlueChip { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool LongTerm { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool Swing { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool Scalp { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool Day { get; set; }     
    }
}