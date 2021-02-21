using System;
using System.Runtime.CompilerServices;
using System.Net.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class Preference
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int? Id { get; set; }

        [Required]
        public RiskLevel RiskLevel { get; set; }

        public DateTime? DateModified { get; set; }

        [Required]
        public double CapitalToRisk { get; set; }
    }
}