using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PlayCore.Core.Repository
{
    public class Specification<TSource> : ISpecification<TSource>
    {
        public List<Expression<Func<TSource, bool>>> Filter { get; private set; }
        public Expression<Func<TSource, object>> OrderBy { get; private set; }
        public Expression<Func<TSource, object>> OrderByDescending { get; private set; }
        public Expression<Func<TSource, object>> Select { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public Specification()
        {
            Filter = new List<Expression<Func<TSource, bool>>>();
        }
        public Specification(Expression<Func<TSource, bool>> baseFilter)
        {
            Filter = new List<Expression<Func<TSource, bool>>>
            {
                baseFilter
            };
        }
        public Specification<TSource> ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            return this;
        }
        public Specification<TSource> ApplyOrderBy(Expression<Func<TSource, object>> expression)
        {
            OrderBy = expression;
            return this;
        }
        public Specification<TSource> ApplyOrderByDescending(Expression<Func<TSource, object>> expression)
        {
            OrderByDescending = expression;
            return this;
        }
        public Specification<TSource> ApplySelect<TResult>(Expression<Func<TSource, TResult>> expression)
        {
            return this;
        }
        public Specification<TSource> AddFilter(Expression<Func<TSource, bool>> expression)
        {
            Filter.Add(expression);
            return this;
        }
    }
}
