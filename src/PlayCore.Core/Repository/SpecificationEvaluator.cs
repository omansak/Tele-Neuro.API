using System.Linq;

namespace PlayCore.Core.Repository
{
    public class SpecificationEvaluator<TSource> where TSource : class
    {
        // TODO: Add Include
        public static IQueryable<TSource> GetQuery(IQueryable<TSource> inputQuery, ISpecification<TSource> specification)
        {
            IQueryable<TSource> query = inputQuery;

            // Where
            foreach (var filter in specification.Filter)
            {
                query = query.Where(filter);
            }

            // OrderBy
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Paging
            if (specification.Skip > -1 && specification.Take > 0)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            return query;
        }
    }
}
