using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.DTO;
using MyBGList.Models;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly ILogger<BoardGamesController> _logger;
        private readonly ApplicationDbContext _context;

        public BoardGamesController(ILogger<BoardGamesController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<RestDTO<BoardGame[]>> Get([FromQuery] RequestDTO<BoardGameDTO> input)
        {
            _logger.LogInformation("Get method started.");

            var query = _context.BoardGames.AsQueryable();

            if (!string.IsNullOrEmpty(input.FilterQuery))
            {
                query = query.Where(b => b.Name.Contains(input.FilterQuery));
            }
            var recordCount = await query.CountAsync();
            query = query.OrderBy($"{input.SortColumn} {input.SortOrder}").Skip(input.PageIndex * input.PageSize).Take(input.PageSize);

            return new RestDTO<BoardGame[]>()
            {
                Data = await query.ToArrayAsync(),
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = recordCount,
                Links = new List<LinkDTO>{
                    new LinkDTO(Url.Action(null, "BoardGames", new {input.PageIndex, input.PageSize}, Request.Scheme)!, "self", "GET")
                }
            };
        }

        [HttpPost(Name = "UpdateBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Post(BoardGameDTO model)
        {
            var boardgame = await _context.BoardGames.Where(boardgame => boardgame.Id == model.Id).FirstOrDefaultAsync();

            if (boardgame != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    boardgame.Name = model.Name;
                }

                if (model.Year.HasValue && model.Year.Value > 0)
                {
                    boardgame.Year = model.Year.Value;
                }

                boardgame.LastModifiedDate = DateTime.Now;
                _context.BoardGames.Update(boardgame);

                await _context.SaveChangesAsync();
            }

            return new RestDTO<BoardGame?>()
            {
                Data = boardgame,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(Url.Action(null, "BoardGames", model, Request.Scheme)!, "self", "POST")
                }
            };
        }

        [HttpDelete(Name = "DeleteBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Delete(int id)
        {
            var boardgame = await _context.BoardGames.Where(boardgame => boardgame.Id == id).FirstOrDefaultAsync();

            if (boardgame != null)
            {
                _context.BoardGames.Remove(boardgame);
                await _context.SaveChangesAsync();
            }

            return new RestDTO<BoardGame?>()
            {
                Data = boardgame,
                Links = new List<LinkDTO> { new LinkDTO(Url.Action(null, "BoardGames", id, Request.Scheme)!, "self", "DELETE") }
            };
        }
    }
}