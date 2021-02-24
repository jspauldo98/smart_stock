using System;
using System.Runtime.CompilerServices;
using System.Net.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class RiskLevel
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int? Id { get; set; }

        [Required]
        public string Risk { get; set; }

        [Required]
        public DateTime DateAdded {get; set; }
    }
}