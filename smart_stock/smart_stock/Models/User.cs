using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class User
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime JoinDate { get; set; }

        [Required]
        public Credentials Credentials { get; set; }

        [Required]
        public Pii Pii { get; set; }
    }
}