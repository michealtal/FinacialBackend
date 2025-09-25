using System.ComponentModel.DataAnnotations.Schema;

namespace FinacialBackend.Model
{
    [Table("Portfolios")]
    public class Portfolio
    {
        public  string  AppUserId { get; set; }    
        public int StockId { get; set; }
        public AppUser AppUser { get; set; } // this is for you as the developer
        public Stock Stock { get; set; }
    }
}
