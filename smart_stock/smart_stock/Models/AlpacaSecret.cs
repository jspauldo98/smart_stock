using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smart_stock.Models
{
    public class AlpacaSecret
    {
        [Column(TypeName = "varchar(200")]
        public string AlpacaKeyId { get; set; }

        [Column(TypeName = "varchar(200")]
        public string AlpacaKey { get; set; }
    }
}