using System.Collections.Generic;
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

        public BaseController(IBaseRepository<TEntity, TContext> service)
        {
            _service = service;
        }

        [HttpGet("Get")]
        public async Task<BaseResponse<TEntity>> Get(int id)
        {
            return new BaseResponse<TEntity>().SetResult(await _service.FindByIdAsync(id));
        }

        [HttpGet("List")]
        public async Task<BaseResponse<IEnumerable<TEntity>>> List()
        {
            var response = new BaseResponse<IEnumerable<TEntity>>()
               .SetResult(await _service.ListAllAsync());
            response.SetTotalCount(response.Result.Count)
               .SetPage(1)
               .SetPageSize(1);
            return response;
        }

        [HttpPost("ListFilter")]
        public async Task<BaseResponse<IEnumerable<TEntity>>> ListFilter(BaseFilterModel baseFilterModel)
        {
            return new BaseResponse<IEnumerable<TEntity>>()
                .SetResult(await _service.ListFilterAsync(baseFilterModel))
                .SetTotalCount(await _service.CountFilterAsync(baseFilterModel, includePaging: false))
                .SetPage(MathExtensions.UpDivision(baseFilterModel.PagingBy.Skip + baseFilterModel.PagingBy.Take,
                    baseFilterModel.PagingBy.Take))
                .SetPageSize(baseFilterModel.PagingBy.Take);
        }

        [HttpGet("Count")]
        public async Task<BaseResponse<int>> Count()
        {
            return new BaseResponse<int>().SetResult(await _service.CountAsync());
        }
        /// <summary>
        /// BaseFilterModel
        /// </summary>
        /// <param name="baseFilterModel"></param>
        /// <returns></returns>
        [HttpPost("CountFilter")]
        public async Task<BaseResponse<int>> CountFilter(BaseFilterModel baseFilterModel)
        {
            return new BaseResponse<int>().SetResult(await _service.CountFilterAsync(baseFilterModel));
        }
    }
}
