using PlayCore.Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace PlayCore.Core.Extension
{
    public static class BaseFilterModelExtensions
    {
        public static IQueryable<TEntity> ToQueryableFromBaseFilter<TEntity>(this IQueryable<TEntity> query, BaseFilterModel baseFilterModel, bool includeFilters = true, bool includePaging = true)
        {
            if (baseFilterModel.SortBy?.Any() == true)
            {
                string sortString = string.Join(",", baseFilterModel.ToQuerySortString());
                query = query.OrderBy(sortString);
            }
            if (includePaging && baseFilterModel.PagingBy is { IsValid: true })
            {
                query = query.Skip(baseFilterModel.PagingBy.Skip).Take(baseFilterModel.PagingBy.Take);
            }
            if (includeFilters)
            {
                string predicate = baseFilterModel.ToQueryFilterString();
                if (!string.IsNullOrEmpty(predicate))
                    query = query.Where(predicate, baseFilterModel.FilterBy.Select(i => i.Value).ToArray());
            }
            return query;
        }

        private static string ToQueryFilterString(this BaseFilterModel baseFilterModel)
        {
            string queryOperator = string.Empty;
            StringBuilder predicateBuilder = new StringBuilder("i=>");
            for (int i = 0; baseFilterModel.FilterBy != null && i < baseFilterModel.FilterBy.Count; i++)
            {
                var filterType = BaseFilterModel.Filter.ToFilterType(baseFilterModel.FilterBy[i].TypeString);

                if (baseFilterModel.FilterBy[i].StartsParentheses)
                    predicateBuilder.Append("(");

                switch (filterType)
                {
                    case BaseFilterModel.FilterType.Contains: // Strings
                    case BaseFilterModel.FilterType.NotContains:
                    case BaseFilterModel.FilterType.Equal: // Int,Date,Strings,Double
                    case BaseFilterModel.FilterType.NotEqual:
                    case BaseFilterModel.FilterType.StartsWith: // Strings
                    case BaseFilterModel.FilterType.EndWith:
                        {
                            string functionName = string.Empty;
                            switch (filterType)
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
                            predicateBuilder.Append($"{queryOperator}i.{baseFilterModel.FilterBy[i].ColumnName}.{functionName}(@{i})");
                            break;
                        }
                    case BaseFilterModel.FilterType.LessThan: // Int,Date,Double
                        predicateBuilder.Append($"i.{baseFilterModel.FilterBy[i].ColumnName} < @{i}");
                        break;
                    case BaseFilterModel.FilterType.LessEqualThan: // Int,Date,Double
                        predicateBuilder.Append($"i.{baseFilterModel.FilterBy[i].ColumnName} <= @{i}");
                        break;
                    case BaseFilterModel.FilterType.GreaterThan: // Int,Date,Double
                        predicateBuilder.Append($"i.{baseFilterModel.FilterBy[i].ColumnName} > @{i}");
                        break;
                    case BaseFilterModel.FilterType.GreaterEqualThan: // Int,Date,Double
                        predicateBuilder.Append($"i.{baseFilterModel.FilterBy[i].ColumnName} >= @{i}");
                        break;
                }

                if (baseFilterModel.FilterBy[i].EndParentheses)
                    predicateBuilder.Append(")");

                if (i + 1 < baseFilterModel.FilterBy.Count)
                {
                    predicateBuilder.Append(baseFilterModel.FilterBy[i].IsAndWithNextFilter ? "&&" : "||");
                }
            }

            return predicateBuilder.Length > 3 ? predicateBuilder.ToString() : null;
        }

        private static IEnumerable<string> ToQuerySortString(this BaseFilterModel baseFilterModel)
        {
            foreach (var item in baseFilterModel.SortBy.Where(i => i.IsValid))
            {
                switch (item.Type)
                {
                    case BaseFilterModel.OrderType.Descending:
                        yield return $"{item.ColumnName} descending";
                        break;
                    default:
                        yield return $"{item.ColumnName} ascending";
                        break;
                }
            }
        }
    }
}
