using Microsoft.AspNetCore.Http;
using PlayCore.Core.CustomException;
using PlayCore.Core.Extension;
using PlayCore.Core.LocalizationString;
using PlayCore.Core.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlayCore.Core.Middleware.GlobalExceptionHandler
{
    public class GlobalExceptionHandler
    {
        private readonly Func<Exception, Exception> _action;
        private readonly RequestDelegate _next;
        private readonly GlobalExceptionHandlerStrings _strings;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
            _strings = new GlobalExceptionHandlerStrings();
        }

        public GlobalExceptionHandler(RequestDelegate next, Func<Exception, Exception> action)
        {
            _next = next;
            _strings = new GlobalExceptionHandlerStrings();
            _action = action;
        }
        public GlobalExceptionHandler(RequestDelegate next, GlobalExceptionHandlerStrings strings)
        {
            _next = next;
            _strings = strings;
        }
        public GlobalExceptionHandler(RequestDelegate next, GlobalExceptionHandlerStrings strings, Func<Exception, Exception> action)
        {
            _next = next;
            _strings = strings;
            _action = action;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UIException ex)
            {
                await this.ErrorResponse(context, ex);
            }
            catch (Exception ex)
            {
                if (_action != null)
                    await this.ErrorResponse(context, _action(ex));
                await this.ErrorResponse(context, ex);
            }
        }

        private async Task ErrorResponse(HttpContext context, Exception ex)
        {
            if (!context.Response.HasStarted)
            {
                BaseResponse baseResponse = new BaseResponse
                {
                    Status =
                    {
                        ResponseType = ResponseTypes.Error,
                        Code = 500,
                        Domain = context.Request.Path,
                        Message = ex.Message,

                    },
                    Result =
                    {
                        Data = _strings.BaseErrorMessage
                    }

                };
                if (ex.Data.HasKey("Result"))
                {
                    baseResponse.Result.Data = ex.Data["Result"];
                }
                if (ex.Data.HasKey("ResultType"))
                {
                    baseResponse.Result.Type = ex.Data["ResultType"].ToString().Split('.').Last();
                }
                if (ex.Data.HasKey("ResultMessage"))
                {
                    baseResponse.Result.Message = ex.Data["ResultMessage"].ToString();
                }
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = baseResponse.Status.Code;
                await context.Response.WriteAsync(baseResponse.ToJson());
            }
        }
    }
}
