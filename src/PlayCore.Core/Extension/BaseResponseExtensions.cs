using PlayCore.Core.Model;

namespace PlayCore.Core.Extension
{
    public static class BaseResponseExtensions
    {
        public static BaseResponse SetResult(this BaseResponse baseResponse, object result)
        {
            baseResponse.Result.Data = result;
            return baseResponse;
        }
        public static BaseResponse SetCode(this BaseResponse baseResponse, int code)
        {
            baseResponse.Status.Code = code;
            return baseResponse;
        }
        public static BaseResponse SetDomain(this BaseResponse baseResponse, string domain)
        {
            baseResponse.Status.Domain = domain;
            return baseResponse;
        }
        public static BaseResponse SetMessage(this BaseResponse baseResponse, string message)
        {
            baseResponse.Result.Message = message;
            return baseResponse;
        }
        public static BaseResponse SetTotalCount(this BaseResponse baseResponse, int count)
        {
            baseResponse.Result.PageInfo.TotalCount = count;
            return baseResponse;
        }
        public static BaseResponse SetPage(this BaseResponse baseResponse, int? page)
        {
            baseResponse.Result.PageInfo.Page = page ?? 1;
            return baseResponse;
        }
        public static BaseResponse SetPageSize(this BaseResponse baseResponse, int? pageSize)
        {
            baseResponse.Result.PageInfo.PageSize = pageSize ?? 1;
            return baseResponse;
        }
    }
}
