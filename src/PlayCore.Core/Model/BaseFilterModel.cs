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
            public bool IsAndWithNextFilter { get; set; }
            public bool StartsParentheses { get; set; }
            public bool EndParentheses { get; set; }

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

    #region unsued
    //public class BaseFilterModel<T> where T : class, new()
    //{
    //    public BaseFilterModel.Paging PagingBy { get; set; }
    //    public List<Sort> SortBy { get; set; }
    //    public List<Filter> FilterBy { get; set; }
    //    public class Sort
    //    {
    //        internal string ColumnName { get; set; }
    //        internal BaseFilterModel.OrderType TypeAsd { get; set; }
    //        public Sort Set<TKey>(Expression<Func<T, TKey>> expression, TKey[] value, BaseFilterModel.OrderType type) where TKey : struct
    //        {
    //            if (expression.Body.NodeType != ExpressionType.MemberAccess)
    //                throw new FormatException("ColumnName");

    //            var body = expression.Body.ToString();
    //            this.ColumnName = body[(body.IndexOf(".", StringComparison.Ordinal) + 1)..];
    //            this.TypeAsd = type;
    //            return this;
    //        }
    //    }
    //    public class Filter
    //    {
    //        internal string ColumnName { get; set; }
    //        internal object Value { get; set; }
    //        internal bool IsAndWithNextFilter { get; set; }
    //        internal BaseFilterModel.FilterType TypeAsd { get; set; }
    //        public Filter Set<TKey>(Expression<Func<T, TKey>> expression, TKey value, BaseFilterModel.FilterType type, bool isAndWithNextFilter = false) where TKey : struct
    //        {
    //            if (expression.Body.NodeType != ExpressionType.MemberAccess)
    //                throw new FormatException("ColumnName");

    //            var body = expression.Body.ToString();
    //            this.ColumnName = body[(body.IndexOf(".", StringComparison.Ordinal) + 1)..];
    //            this.Value = value as object;
    //            this.IsAndWithNextFilter = isAndWithNextFilter;
    //            this.TypeAsd = type;
    //            return this;
    //        }

    //    }

    //    public BaseFilterModel ToBaseFilterModel()
    //    {
    //        var result = new BaseFilterModel
    //        {
    //            PagingBy = this.PagingBy,
    //            SortBy = this.SortBy
    //                .Select(i => new BaseFilterModel.Sort
    //                {
    //                    ColumnName = i.ColumnName,
    //                    TypeAsd = i.TypeAsd
    //                })
    //                .ToList(),
    //            FilterBy = this.FilterBy
    //                .Select(i => new BaseFilterModel.Filter
    //                {
    //                    Value = i.Value,
    //                    ColumnName = i.ColumnName,
    //                    IsAndWithNextFilter = i.IsAndWithNextFilter,
    //                    TypeString = BaseFilterModel.Filter.FilterTypeMap.First(j => j.Value == i.TypeAsd).Key
    //                })
    //                .ToList()
    //        };
    //        return result;
    //    }
    //}
    #endregion

}
