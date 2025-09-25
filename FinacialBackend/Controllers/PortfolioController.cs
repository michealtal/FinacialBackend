using FinacialBackend.Dtos.Comment;
using FinacialBackend.Extension;
using FinacialBackend.Interfaces;
using FinacialBackend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinacialBackend.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IFMPServices _fmpServices;
        public PortfolioController(UserManager<AppUser> userManager,
            IStockRepository stockRepo,
            IPortfolioRepository portfolioRepo,
            IFMPServices fmpService
            )
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _portfolioRepo = portfolioRepo;
            _fmpServices = fmpService;
        }

        [HttpGet]
        [Authorize]

        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);

            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            var stock = await _stockRepo.GetBySymbolAsync(symbol);


            if (stock == null)
            {
                stock = await _fmpServices.FindStockBySymbolAsync(symbol);
                if (stock == null)
                {
                    return BadRequest("Stock does not exist "); ;

                }
                else
                {
                    await _stockRepo.CreateAsync(stock);
                }
            }
            if (stock == null) return BadRequest("Stock Not Found");
          

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);

                if (userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower())) return BadRequest("Cannot add same stock to portfolio");

                var portfolioModel = new Portfolio
                {
                    StockId = stock.Id,
                    AppUserId = appUser.Id,

                };
                await _portfolioRepo.CreateAsync(portfolioModel);

                if (portfolioModel == null)
                {
                    return StatusCode(500, "Could not create ");
                }
                else
                {
                    return Created();
                }
            }

            [HttpDelete]
            [Authorize]
            public async Task<IActionResult> DeletePortfolio(string symbol)
            {
                var username = User.GetUsername();
                var appUser = await _userManager.FindByNameAsync(username);

                var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);

                var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();

                if (filteredStock.Count() == 1)
                {
                    await _portfolioRepo.DeletePortfolio(appUser, symbol);
                }
                else
                {
                    return BadRequest("Stock is not in your portfolio");
                }
                return Ok();
            }

            //OR

            //public async Task<IActionResult> DeletePortfolio(string symbol)
            //{       
            //    var username = User.GetUsername();
            //    var appUser = await _userManager.FindByNameAsync(username);

            //    var deleted = await _portfolioRepo.DeletePortfolio(appUser, symbol);

            //    if (deleted == null)
            //    {
            //        return BadRequest("Stock is not in your portfolio");   
            //    }

            //    return Ok("Stock deleted successfully");
            //}

        }
    }

