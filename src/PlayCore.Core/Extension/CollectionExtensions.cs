using System;
using System.Collections;

namespace PlayCore.Core.Extension
{
    public static class CollectionExtensions
    {
        public static bool HasKey(this IDictionary collection, string key)
        {
            return collection.Count > 0 && collection[key] != null;
        }
        public static bool IfHasKey(this IDictionary collection, string key, Action action)
        {
            if (collection.HasKey(key))
            {
                action();
                return true;
            }
            return false;
        }
    }
}
