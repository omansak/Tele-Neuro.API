using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using TeleNeuro.Service.UtilityService;

namespace TeleNeuro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UtilityController
    {
        private readonly IUtilityService _utilityService;

        public UtilityController(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        [HttpGet]
        public async Task<BaseResponse> ListExercisePropertyDefinitions()
        {
            return new BaseResponse()
                .SetResult(await _utilityService.ListExercisePropertyDefinitions());
        }
    }
}
