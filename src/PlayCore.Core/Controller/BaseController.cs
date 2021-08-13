using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayCore.Core.Extension;
using PlayCore.Core.Model;
using PlayCore.Core.Repository;

namespace PlayCore.Core.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity, TContext> : ControllerBase where TContext : DbContext where TEntity : class
    {
        private readonly IBaseRepository<TEntity, TContext> _service;
        private readonly BaseResponse _baseResponse;

        public BaseController(IBaseRepository<TEntity, TContext> service)
        {
            _service = service;
            _baseResponse = new BaseResponse();
        }

        [HttpGet("Get")]
        public async Task<BaseResponse> Get(int id)
        {
            return _baseResponse.SetResult(await _service.FindByIdAsync(id));
        }

        [HttpGet("List")]
        public async Task<BaseResponse> List()
        {
            return _baseResponse
                .SetResult(await _service.ListAllAsync())
                .SetTotalCount(_baseResponse.Result.Count)
                .SetPage(1)
                .SetPageSize(1);
        }

        [HttpPost("ListFilter")]
        public async Task<BaseResponse> ListFilter(BaseFilterModel baseFilterModel)
        {
            return _baseResponse
                .SetResult(await _service.ListFilterAsync(baseFilterModel))
                .SetTotalCount(await _service.CountFilterAsync(baseFilterModel, includePaging: false))
                .SetPage(MathExtensions.UpDivision(baseFilterModel.PagingBy.Skip + baseFilterModel.PagingBy.Take,
                    baseFilterModel.PagingBy.Take))
                .SetPageSize(baseFilterModel.PagingBy.Take);
        }

        [HttpGet("Count")]
        public async Task<BaseResponse> Count()
        {
            return _baseResponse.SetResult(await _service.CountAsync());
        }

        [HttpPost("CountFilter")]
        public async Task<BaseResponse> CountFilter(BaseFilterModel baseFilterModel)
        {
            return _baseResponse.SetResult(await _service.CountFilterAsync(baseFilterModel));
        }
    }
}
