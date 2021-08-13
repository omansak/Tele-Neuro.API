using System;
using System.Collections;
using System.Linq;
using PlayCore.Core.Extension;

namespace PlayCore.Core.Model
{
    // TODO : Maybe DI
    public class BaseResponse
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
            public object Data { get; set; } 
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

            public int TotalCount { get; set; }
            public int PageSize { get; set; }
            public int Page { get; set; } = 1;
            public int TotalPage => MathExtensions.UpDivision(TotalCount, PageSize);
        }

        public BaseResponse()
        {
            this.Status = new ResponseStatus
            {
                Code = 200
            };
            this.Result = new ResponseResult();
        }
    }
    public enum ResponseTypes
    {

        Success = 0,
        Warning = 1,
        Error = 2
    }
}
