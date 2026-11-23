using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FTShirts.Web.Data
{
    // A database query where every filter is optional: a filter is only added when the user
    // actually used it (typed a team, ticked "only offers", selected a tag...).
    // That "skip it if it is empty" check is written once, here, instead of once for every filter.
    // It is generic: T is the type of the rows, so it works with any table (shirts, users...).
    public class OptionalQuery<T>
    {
        private IQueryable<T> _query;

        public OptionalQuery(IQueryable<T> query)
        {
            _query = query;
        }

        // Adds the condition to the query only when the filter is used
        public OptionalQuery<T> WhereIf(bool isUsed, Expression<Func<T, bool>> condition)
        {
            if (isUsed)
            {
                _query = _query.Where(condition);
            }
            return this;
        }

        // Runs the query. The filtering happens in the database (in SQL),
        // so only the rows that match are loaded, not the whole table.
        public Task<List<T>> ToListAsync()
        {
            return _query.ToListAsync();
        }
    }

    public static class OptionalQueryExtensions
    {
        // Generic method: turns a query of any type T into an OptionalQuery<T>.
        // The compiler works out T by itself: _context.CatalogueItems.ToOptionalQuery() gives an OptionalQuery<CatalogueItem>.
        public static OptionalQuery<T> ToOptionalQuery<T>(this IQueryable<T> query)
        {
            return new OptionalQuery<T>(query);
        }
    }
}