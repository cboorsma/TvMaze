using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Model.Services;

namespace Model.Controllers
{
    [Route("[controller]")]
    public class TvMazeController : Controller
    {
        private readonly ITvMazeService _tvMazeServiceService;

        public TvMazeController(ITvMazeService tvMazeServiceService)
        {
            _tvMazeServiceService = tvMazeServiceService;
        }

        //Action to get the info from TXMaze API and store in database
        [HttpPost]
        public async Task<IActionResult> FillDataBaseAsync()
        {
            var result = await _tvMazeServiceService.GetTvMazeResults();
            return Ok(result);
        }

        //Action to get the paginated results from the database
        [HttpGet]
        public async Task<IActionResult> GetResults([FromQuery]int page, [FromQuery] int size)
        {
            if (size <= 0) size = 50;
            if (page < 0) page = 0;
            var result = await _tvMazeServiceService.GetDatabaseTvMazeResults(page,size);
            return Ok(result);
        }
    }
}