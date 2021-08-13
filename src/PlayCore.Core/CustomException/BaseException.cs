using System;
using System.Runtime.Serialization;

namespace PlayCore.Core.CustomException
{
    public class BaseException : SystemException
    {
        public BaseException() : base("Exception of type 'BaseException' was thrown.")
        { }
        public BaseException(string message) : base(message)
        { }
        public BaseException(string message, Exception innerException) : base(message, innerException)
        { }
        public BaseException(Exception innerException) : base(innerException.Message, innerException)
        { }
        protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
        public BaseException SetResultMessage(string message)
        {
            base.Data.Add("ResultMessage", message);
            return this;
        }
        public BaseException SetResult<TResult>(TResult result)
        {
            base.Data.Add("Result", result);
            return this;
        }
        public BaseException SetResultType(string type)
        {
            base.Data.Add("ResultType", type);
            return this;
        }

    }
}
