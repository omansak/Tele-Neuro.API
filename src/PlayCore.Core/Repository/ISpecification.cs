using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PlayCore.Core.Repository
{
    public interface ISpecification<TSource>
    {
        List<Expression<Func<TSource, bool>>> Filter { get; }
        Expression<Func<TSource, object>> OrderBy { get; }
        Expression<Func<TSource, object>> OrderByDescending { get; }
        int Take { get; }
        int Skip { get; }
    }
}
