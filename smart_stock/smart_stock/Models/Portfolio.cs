using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class Portfolio
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Column(TypeName = "varchar(50)")]
        public double Profit { get; set; }

        [Column(TypeName = "varchar(50)")]
        public double Amount { get; set; }

        [Column(TypeName = "varchar(50)")]
        public double Loss { get; set; }

        [Column(TypeName = "varchar(50)")]
        public double Net { get; set; }

        [Column(TypeName = "varchar(50)")]
        public double Invested { get; set; }

        [Column(TypeName = "varchar(50)")]
        public double Cash { get; set; }
    }
}