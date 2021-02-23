using System;
using System.Runtime.CompilerServices;
using System.Net.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class Sector
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int? Id { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean InformationTechnology { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean HealthCare { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean Financials { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean ConsumerDiscretionary { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean Communication { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean Industrials { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean ConsumerStaples { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean Energy { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean Utilities { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean RealEstate { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public Boolean Materials { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateAdded { get; set; }
    }
}