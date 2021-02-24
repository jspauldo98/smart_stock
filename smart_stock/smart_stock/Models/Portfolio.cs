using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class Portfolio
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int? Id { get; set; }

        [Required]
        public User User { get; set; }

        [Column(TypeName = "double")]
        public double Profit { get; set; }

        [Column(TypeName = "double")]
        public double Loss { get; set; }

        [Column(TypeName = "double")]
        public double Net { get; set; }
    }
}