using Newtonsoft.Json;

namespace PlayCore.Core.Extension
{
    public static class JsonExtensions
    {
        // TODO : Pretty flag
        public static string ToJson(this object obj, bool pretty = true, bool ignoreNull = false)
        {
            return JsonConvert.SerializeObject(obj, pretty ? Formatting.Indented : Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include
            });
        }
    }
}
