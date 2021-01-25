using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class Pii
    {
        //Need to fix database schema, snake case mapping isn't ideal but variables must match
        //column names.
        [Key]
        [Column(TypeName = "int(64)")]
        public int id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string F_Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string L_Name { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Dob { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Phone { get; set; }
    }
}