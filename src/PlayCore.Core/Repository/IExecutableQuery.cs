using System.Data;

namespace PlayCore.Core.Repository
{
    public interface IExecutableQuery
    {
        string GetCommandText();
        string GetPreparedCommandText();
        CommandType GetCommandType();
        object[] GetParameters();
    }
}
