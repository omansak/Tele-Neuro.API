using System;
using System.Collections.Generic;
using System.Text;
using PlayCore.Core.Extension;

namespace PlayCore.Core.Model
{
    public class BaseFilterModel
    {
        public BaseFilterModel()
        {
            this.FilterBy = new List<Filter>();
            this.PagingBy = new Paging();
            this.SortBy = new Sort();
        }
        public Sort SortBy { get; set; }
        public Paging PagingBy { get; set; }
        public List<Filter> FilterBy { get; set; }
        public class Sort
        {
            private OrderType[] _type { get; set; }
            public OrderType[] Type
            {
                get => this._type;
                set => this._type = value;
            }
            private string[] _columnName { get; set; }
            public string[] ColumnName
            {
                get => this._columnName;
                set => this._columnName = value;
            }
            public bool IsValid => _columnName != null && _type != null;
        }
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
        public class Filter
        {
            public FilterType? Type { get; private set; }
            public string ColumnName { get; set; }
            public string TypeString
            {
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        switch (value.ToLower())
                        {
                            case "contains":
                            case "%%":
                                Type = FilterType.Contains;
                                break;
                            case "!%%":
                                Type = FilterType.NotContains;
                                break;
                            case "=":
                            case "==":
                                Type = FilterType.Equal;
                                break;
                            case "!=":
                                Type = FilterType.NotEqual;
                                break;
                            case "startswith":
                            case "%=":
                                Type = FilterType.StartsWith;
                                break;
                            case "endswith":
                            case "=%":
                                Type = FilterType.EndWith;
                                break;
                            case "<":
                                Type = FilterType.LessThan;
                                break;
                            case "<=":
                                Type = FilterType.LessEqualThan;
                                break;
                            case ">":
                                Type = FilterType.GreaterThan;
                                break;
                            case ">=":
                                Type = FilterType.GreaterEqualThan;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(value), value, "Operator is not defined");
                        }
                    }
                }
            }
            public object[] Value { get; set; }
            public bool IsAndWithNextFilter { get; set; }
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
