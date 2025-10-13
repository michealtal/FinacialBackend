using FinacialBackend.Dtos.Comment;
using FinacialBackend.Extension;
using FinacialBackend.Helpers;
using FinacialBackend.Interfaces;
using FinacialBackend.Mapper;
using FinacialBackend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace FinacialBackend.Controllers
{
    [Route("api/comment")]
    [ApiController] 
    public class CommentControllers : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFMPServices _fmpServices;

        public CommentControllers(ICommentRepository commentRepo, IStockRepository stockRepo, UserManager<AppUser> userManager, IFMPServices fmpService)
        {
          _commentRepo = commentRepo;  
            _stockRepo = stockRepo;
            _userManager = userManager;
            _fmpServices = fmpService;

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Getall([FromQuery]CommentQueryObject queryObject)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var comments = await _commentRepo.GetAllAsync( queryObject);  
             
            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        } 

        [HttpGet("{id:int}")]  
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState); 

            var comment = await _commentRepo.GetByIdAsync(id);
               
            if (comment == null)   
            {
                return NotFound();
            } 
            return Ok(comment.ToCommentDto());
        }
               
        [HttpPost("symbol/{symbol:alpha}")]
        // [Route("{stockId:int}")] when you are finding by stockId and you are not dataSeeding from external api
        [Authorize] // ensure only logged-in users can comment
        public async Task<IActionResult> Create([FromRoute] string symbol, [FromBody]CreateCommentDto commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if stock exists BEFORE DATA SEEDING
            //if (!await _stockRepo.StockExist(stockId))
            //    return BadRequest("Stock does not exist");

            // AFTER DATA SEEDING  check for the stock 

            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                stock = await _fmpServices.FindStockBySymbolAsync(symbol);
                if(stock == null)
                {
                    return BadRequest("Stock does not exist "); ;

                }
                else
                {
                    await _stockRepo.CreateAsync(stock);
                }
            }

            // Get user from JWT token
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("JWT username missing");
                return Unauthorized("User not found in token");
            }
            var appUser = await _userManager.FindByNameAsync(username);

            if (appUser == null)
                return Unauthorized("User not found");

            // Map to comment model
            var commentModel = commentDto.ToCommentFromCreate(stock.Id);
            commentModel.AppUserId = appUser.Id;

            await _commentRepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }


        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var commentModel = await _commentRepo.DeleteAsync(id);
            if (commentModel == null)
            {
                return NotFound("Comment does Not Exist");                
            }
            return Ok(commentModel);
        }

        [HttpPut]
        [Route("{id:int}")]

        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());

            if (comment == null)
            {
                return NotFound("Comment Not Found");
            }
            return Ok(comment.ToCommentDto());
        }

    }
} 
