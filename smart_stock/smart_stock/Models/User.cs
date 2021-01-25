using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class User
    {
        //Need to fix database schema, snake case mapping isn't ideal but variables must match
        //column names.
        [Key]
        [Column(TypeName = "int(64)")]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Join_Date { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date_Added { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date_Confirmed { get; set; }

        [Required]
        public Pii Pii { get; set; }

        [Required]
        public Credential Credential { get; set; }
    }
}