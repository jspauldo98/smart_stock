using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class Credential
    {
        [Key]
        [Column(TypeName = "int(64)")]
        public int? Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Username { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Password { get; set; }
    }
}