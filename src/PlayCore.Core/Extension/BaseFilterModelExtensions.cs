using System;
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
                string sortString = string.Join(",", ToQuerySortString(baseFilterModel.SortBy));
                query = query.OrderBy(sortString);
            }
            if (includePaging && baseFilterModel.PagingBy is { IsValid: true })
            {
                query = query.Skip(baseFilterModel.PagingBy.Skip).Take(baseFilterModel.PagingBy.Take);
            }
            if (includeFilters)
            {
                string predicate = ToQueryFilterString(baseFilterModel.FilterBy);
                if (!string.IsNullOrEmpty(predicate))
                    query = query.Where(predicate, baseFilterModel.FilterBy.Select(i => i.Value).ToArray());
            }
            return query;
        }

        private static string ToQueryFilterString(List<BaseFilterModel.Filter> filters, int lambdaIterator = 0)
        {
            string GetLambdaKey(int iterator)
            {
                int repeatCount = (iterator / 26);
                char key = (char)((iterator % 26) + 97);
                return string.Empty.PadLeft(repeatCount + 1, key);
            }

            if (filters != null)
            {
                string lambdaKey = GetLambdaKey(lambdaIterator);
                string queryOperator = string.Empty;
                StringBuilder predicateBuilder = new StringBuilder($"{lambdaKey}=>");
                for (int i = 0; i < filters.Count; i++)
                {
                    var filterType = BaseFilterModel.Filter.ToFilterType(filters[i].TypeString);

                    if (filters[i].FilterConfig?.StartsParentheses == true)
                        predicateBuilder.Append("(");

                    string predicate = string.Empty;
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

                                predicate = $"{queryOperator}{{0}}{(filters[i].FilterConfig?.IsArray == true ? string.Empty : $".{filters[i].ColumnName}")}.{functionName}(@{i})";
                                break;
                            }
                        case BaseFilterModel.FilterType.LessThan: // Int,Date,Double
                            predicate = $"{{0}}.{(filters[i].FilterConfig?.IsArray == true ? string.Empty : $".{filters[i].ColumnName}")} < @{i}";
                            break;
                        case BaseFilterModel.FilterType.LessEqualThan: // Int,Date,Double
                            predicate = $"{{0}}.{(filters[i].FilterConfig?.IsArray == true ? string.Empty : $".{filters[i].ColumnName}")} <= @{i}";
                            break;
                        case BaseFilterModel.FilterType.GreaterThan: // Int,Date,Double
                            predicate = $"{{0}}.{(filters[i].FilterConfig?.IsArray == true ? string.Empty : $".{filters[i].ColumnName}")} > @{i}";
                            break;
                        case BaseFilterModel.FilterType.GreaterEqualThan: // Int,Date,Double
                            predicate = $"{{0}}.{(filters[i].FilterConfig?.IsArray == true ? string.Empty : $".{filters[i].ColumnName}")} >= @{i}";
                            break;
                    }

                    if (filters[i].FilterConfig?.IsArray == true)
                    {
                        string charLambdaKey = GetLambdaKey(lambdaIterator + 1);
                        predicateBuilder.Append($"{lambdaKey}.{filters[i].ColumnName}.Any({charLambdaKey}=>{string.Format(predicate, charLambdaKey)})");
                    }
                    else
                        predicateBuilder.Append($"{string.Format(predicate, lambdaKey)}");

                    if (filters[i].FilterConfig?.EndParentheses == true)
                        predicateBuilder.Append(")");

                    if (i + 1 < filters.Count)
                    {
                        predicateBuilder.Append(filters[i].FilterConfig?.IsAndNext == true ? "&&" : "||");
                    }
                }

                if (predicateBuilder.Length > 3)
                    return predicateBuilder.ToString();
            }

            return null;
        }

        private static IEnumerable<string> ToQuerySortString(List<BaseFilterModel.Sort> sorts)
        {
            foreach (var item in sorts)
            {
                if (item.IsValid)
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
}
