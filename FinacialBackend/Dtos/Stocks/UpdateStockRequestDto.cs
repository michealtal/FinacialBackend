using System.ComponentModel.DataAnnotations;

namespace FinacialBackend.Dtos.Stocks
{
    public class UpdateStockRequestDto
    {

        [Required]
        [MinLength(1, ErrorMessage = "Symbol is required")]
        [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 characters")]
        public string Symbol { get; set; } = string.Empty;
        [Required]
        [MaxLength(100, ErrorMessage = "Company Name cannot be over 100 chracters")]
        public string CompanyName { get; set; } = string.Empty;
        [Required]
        [Range(1, 10000000000)]
        public decimal Purchase { get; set; }
        [Required]
        [Range(0.001, 100)]
        public decimal LastDiv { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Industry cannot be over 10 chracter")]
        public string Industry { get; set; } = string.Empty;
        [Range(1, 1000000000)]
        public long MarketCap { get; set; }
    }
}
