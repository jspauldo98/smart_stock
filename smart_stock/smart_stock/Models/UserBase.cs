using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class UserBase
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int id { get; set; }

        [Required]
        public User user { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateAdded { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateConfirmed { get; set; }
    }
}