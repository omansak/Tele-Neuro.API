using System;
using System.Collections.Generic;

namespace PlayCore.Core.Model
{
    public class BaseFilterModel
    {
        public Paging PagingBy { get; set; }
        public List<Sort> SortBy { get; set; }
        public List<Filter> FilterBy { get; set; }
        // For Swagger
        public class Paging
        {
            private int _take;
            public int Take
            {
                get => this._take;
                set => this._take = value;
            }
            private int _skip;
            public int Skip
            {
                get => this._skip;
                set => this._skip = value;
            }
            public bool IsValid => _take > 0 && _skip > -1;
        }
        public class Sort
        {
            public OrderType Type { get; set; }
            public string ColumnName { get; set; }
            public bool IsValid => ColumnName != null;
        }
        public class Filter
        {
            public string ColumnName { get; set; }
            public string TypeString { get; set; }
            public object Value { get; set; }
            public FilterConfig FilterConfig { get; set; }
            public List<Filter> Children { get; set; }
            public static Dictionary<string, FilterType> FilterTypeMap = new()
            {
                // Contains
                { "%%", FilterType.Contains },
                { "contains", FilterType.Contains },
                // Not Contains
                { "!%%", FilterType.NotContains },
                { "!contains", FilterType.NotContains },
                // Equal
                { "=", FilterType.Equal },
                { "==", FilterType.Equal },
                { "equal", FilterType.Equal },
                // Not Equal
                { "!=", FilterType.NotEqual },
                { "!equal", FilterType.NotEqual },
                // StartsWith
                { "startswith", FilterType.StartsWith },
                { "%=", FilterType.StartsWith },
                // EndWith
                { "endswith", FilterType.EndWith },
                { "=%", FilterType.EndWith },
                // LessThan
                { "<", FilterType.LessThan },
                // LessEqualThan
                { "<=", FilterType.LessEqualThan },
                // LessThan
                { ">", FilterType.GreaterThan },
                // LessEqualThan
                { ">=", FilterType.GreaterEqualThan }
            };
            public static FilterType ToFilterType(string value)
            {
                if (FilterTypeMap.TryGetValue(value, out var type))
                    return type;
                throw new ArgumentOutOfRangeException(nameof(TypeString));
            }
        }
        public class FilterConfig
        {
            public bool IsAndNext { get; set; }
            public bool StartsParentheses { get; set; }
            public bool EndParentheses { get; set; }
            public bool IsArray { get; set; }
        }
        public enum OrderType
        {
            Ascending,
            Descending
        }
        public enum FilterType
        {
            Contains,
            NotContains,
            Equal,
            NotEqual,
            StartsWith,
            EndWith,
            LessThan,
            LessEqualThan,
            GreaterThan,
            GreaterEqualThan
        }
    }
}
