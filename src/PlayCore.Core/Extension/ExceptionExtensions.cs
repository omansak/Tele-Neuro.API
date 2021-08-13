using System;

namespace PlayCore.Core.Extension
{
    public static class ExceptionExtensions
    {
        public static Exception SetResultType(this Exception ex, string typeName)
        {
            ex.Data.Add("ResultType", typeName);
            return ex;
        }
        public static Exception SetResultMessage(this Exception ex, string message)
        {
            ex.Data.Add("ResultMessage", message);
            return ex;
        }
        public static Exception SetResult(this Exception ex, object result)
        {
            ex.Data.Add("Result", result);
            return ex;
        }
    }
}
