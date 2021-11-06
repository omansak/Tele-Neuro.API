using System;
using System.Collections;
using System.Linq;

namespace PlayCore.Core.Model
{
    // TODO : Maybe DI
    public class BaseResponse<T>
    {
        public ResponseStatus Status { get; set; }
        public ResponseResult Result { get; set; }
        public class ResponseStatus
        {
            public ResponseTypes ResponseType { private get; set; }
            public string Response => this.ResponseType.ToString();
            public bool Success
            {
                get
                {
                    switch (this.ResponseType)
                    {
                        case ResponseTypes.Error:
                            return false;
                        case ResponseTypes.Success:
                        case ResponseTypes.Warning:
                        default:
                            return true;
                    }
                }
            }
            public int Code { get; set; }
            public static DateTime Time => DateTime.Now;
            public string Domain { get; set; }
            public string Message { get; set; }
        }
        public class ResponseResult
        {
            public string Message { get; set; }
            public string Kind => this.Data?.GetType().ToString().Split('.').Last(); // TODO : Remove
            public T Data { get; set; }
            public string Type { get; set; }
            public int Count
            {
                get
                {
                    if (this.Data is ICollection collection)
                    {
                        return collection.Count;
                    }
                    return this.Data != null ? 1 : 0;
                }
            }

            public PageInfo PageInfo { get; set; }

            public ResponseResult()
            {
                this.PageInfo = new PageInfo();
            }
        }

        public BaseResponse()
        {
            this.Status = new ResponseStatus
            {
                Code = 200
            };
            this.Result = new ResponseResult();
        }

        public BaseResponse<T> SetResult(T result)
        {
            this.Result.Data = result;
            return this;
        }
        public BaseResponse<T> SetCode(int code)
        {
            this.Status.Code = code;
            return this;
        }
        public BaseResponse<T> SetDomain(string domain)
        {
            this.Status.Domain = domain;
            return this;
        }
        public BaseResponse<T> SetMessage(string message)
        {
            this.Result.Message = message;
            return this;
        }
        public BaseResponse<T> SetTotalCount(int count)
        {
            this.Result.PageInfo.TotalCount = count;
            return this;
        }
        public BaseResponse<T> SetPage(int? page)
        {
            this.Result.PageInfo.Page = page ?? 1;
            return this;
        }
        public BaseResponse<T> SetPageSize(int? pageSize)
        {
            this.Result.PageInfo.PageSize = pageSize ?? 1;
            return this;
        }
    }

    public enum ResponseTypes
    {

        Success = 0,
        Warning = 1,
        Error = 2
    }
}
