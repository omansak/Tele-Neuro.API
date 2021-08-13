using PlayCore.Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace PlayCore.Core.Extension
{
    public static class BaseFilterModelExtensions
    {
        public static IQueryable<TEntity> ToLinqFromBaseFilter<TEntity>(this IQueryable<TEntity> query, BaseFilterModel baseFilterModel, bool includeFilters = true, bool includePaging = true)
        {
            if (baseFilterModel.SortBy != null && baseFilterModel.SortBy.IsValid)
            {
                string sortString = string.Join(",", baseFilterModel.ToQuerySortString());
                query = query.OrderBy(sortString);
            }
            if (includePaging && baseFilterModel.PagingBy != null && baseFilterModel.PagingBy.IsValid)
            {
                query = query.Skip(baseFilterModel.PagingBy.Skip).Take(baseFilterModel.PagingBy.Take);
            }
            if (includeFilters)
            {
                string predicate = baseFilterModel.ToQueryFilterString();
                if (!string.IsNullOrEmpty(predicate))
                    query = query.Where(predicate);
            }
            return query;
        }

        private static string ToQueryFilterString(this BaseFilterModel baseFilterModel)
        {
            string queryOperator = string.Empty;
            StringBuilder predicateBuilder = new StringBuilder("i=>");
            for (int i = 0; baseFilterModel.FilterBy != null && i < baseFilterModel.FilterBy.Count; i++)
            {
                switch (baseFilterModel.FilterBy[i].Type)
                {
                    case BaseFilterModel.FilterType.Contains: // Strings
                    case BaseFilterModel.FilterType.NotContains:
                    case BaseFilterModel.FilterType.Equal: // Int,Date,Strings,Double
                    case BaseFilterModel.FilterType.NotEqual:
                    case BaseFilterModel.FilterType.StartsWith: // Strings
                    case BaseFilterModel.FilterType.EndWith:
                        {
                            BaseFilterModel.FilterType type = baseFilterModel.FilterBy[i].Type.Value;
                            string functionName = string.Empty;
                            switch (type)
                            {
                                case BaseFilterModel.FilterType.Contains:
                                    {
                                        functionName = "Contains";
                                        queryOperator = "";
                                        break;
                                    }
                                case BaseFilterModel.FilterType.NotContains:
                                    {
                                        functionName = "EndsWith";
                                        queryOperator = "!";
                                        break;
                                    }
                                case BaseFilterModel.FilterType.Equal:
                                    {
                                        functionName = "Equals";
                                        queryOperator = "";
                                        break;
                                    }
                                case BaseFilterModel.FilterType.NotEqual:
                                    {
                                        functionName = "Equals";
                                        queryOperator = "!";
                                        break;
                                    }
                                case BaseFilterModel.FilterType.StartsWith:
                                    {
                                        functionName = "StartsWith";
                                        queryOperator = "";
                                        break;
                                    }
                                case BaseFilterModel.FilterType.EndWith:
                                    {
                                        functionName = "EndsWith";
                                        queryOperator = "";
                                        break;
                                    }

                            }
                            predicateBuilder.Append($"new[]{{{string.Join(",", ToNormalizeValues(baseFilterModel.FilterBy[i].Value))}}}.Any(_j_{i} => {queryOperator}i.{baseFilterModel.FilterBy[i].ColumnName}.{functionName}(_j_{i}))");
                            break;
                        }
                    case BaseFilterModel.FilterType.LessThan: // Int,Date,Double
                        predicateBuilder.Append($"i.{baseFilterModel.FilterBy[i].ColumnName} < {baseFilterModel.FilterBy[i].Value.First()}");
                        break;
                    case BaseFilterModel.FilterType.LessEqualThan: // Int,Date,Double
                        predicateBuilder.Append($"i.{baseFilterModel.FilterBy[i].ColumnName} <= {baseFilterModel.FilterBy[i].Value.First()}");
                        break;
                    case BaseFilterModel.FilterType.GreaterThan: // Int,Date,Double
                        predicateBuilder.Append($"i.{baseFilterModel.FilterBy[i].ColumnName} > {baseFilterModel.FilterBy[i].Value.First()}");
                        break;
                    case BaseFilterModel.FilterType.GreaterEqualThan: // Int,Date,Double
                        predicateBuilder.Append($"i.{baseFilterModel.FilterBy[i].ColumnName} >= {baseFilterModel.FilterBy[i].Value.First()}");
                        break;
                }
                if (i + 1 < baseFilterModel.FilterBy.Count)
                {
                    predicateBuilder.Append(baseFilterModel.FilterBy[i].IsAndWithNextFilter ? "&&" : "||");
                }
            }

            return predicateBuilder.Length > 3 ? predicateBuilder.ToString() : null;
        }

        private static IEnumerable<object> ToNormalizeValues(object[] values)
        {
            foreach (var item in values)
            {
                if (item is string)
                {
                    yield return "\"" + item + "\"";
                }
                else
                {
                    yield return item;
                }
            }
        }
        private static IEnumerable<string> ToQuerySortString(this BaseFilterModel baseFilterModel)
        {
            for (int i = 0; i < baseFilterModel.SortBy.ColumnName.Length; i++)
            {
                switch (baseFilterModel.SortBy.Type[i])
                {
                    case BaseFilterModel.OrderType.DESC:
                        yield return $"{baseFilterModel.SortBy.ColumnName[i]} descending";
                        break;
                    default:
                        yield return $"{baseFilterModel.SortBy.ColumnName[i]} ascending";
                        break;
                }
            }
        }
    }
}
