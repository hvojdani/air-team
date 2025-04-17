using AirTeamApi.Services.Contract;
using AirTeamApi.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace AirTeamApi.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AirTeamController : ControllerBase
    {
        private readonly IAirTeamDataService _airTeamService;
        private readonly ILogger<AirTeamController> _logger;
        public AirTeamController(IAirTeamDataService airTeamService, ILogger<AirTeamController> logging)
        {
            _airTeamService = airTeamService;
            _logger = logging;
        }

        /// <summary>
        /// test serilog seq logging in production
        /// </summary>
        /// <param name="Message">simple text to log as warning message</param>
        /// <returns></returns>
        [HttpGet("logTest")]
        public StatusCodeResult TestLog([Required(AllowEmptyStrings = false), MaxLength(100), RegularExpression(@"^[a-zA-Z0-9.,\-_\s]+$")] string message)
        {
            _logger.LogWarning($"user log text is: ${message}");
            return StatusCode(201);
        }

        /// <summary>
        /// Search AirTeamImages with specified keyword
        /// </summary>
        /// <param name="keyword">keyword to search</param>
        /// <remarks>
        /// /AirTeam/Search?keyword=iriaf
        /// </remarks>
        /// <returns>json array of search results</returns>
        /// <response code="200">if every thing goes well</response>
        /// <response code="400">If keyword not supplied and has length less than 3</response>     
        [HttpGet("Search")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IEnumerable<ImageDto>> Search([MinLength(3), MaxLength(25), Required(AllowEmptyStrings = false), RegularExpression(@"^[a-zA-Z0-9.,\-_\s]+$"), FromQuery] string keyword)
        {
            if (keyword == null)
                throw new ArgumentNullException(keyword);

            if (keyword.Trim().Length < 3)
                return new List<ImageDto>();

            var result = await _airTeamService.SearchByKeyword(keyword);
            return result;
        }
    }
}
