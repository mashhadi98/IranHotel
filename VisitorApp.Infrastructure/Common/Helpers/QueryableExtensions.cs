using System.Linq.Expressions;

namespace VisitorApp.Infrastructure.Common.Helpers;

public static class QueryableExtensions
{
    // Simple dynamic orderby by property name (Used for sorting by CreatedAt)
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName, bool ascending = true)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) return source;
        var param = Expression.Parameter(typeof(T), "x");
        var prop = Expression.PropertyOrField(param, propertyName);
        var lambda = Expression.Lambda(prop, param);
        string method = ascending ? "OrderBy" : "OrderByDescending";
        var result = Expression.Call(typeof(Queryable), method, new Type[] { typeof(T), prop.Type }, source.Expression, Expression.Quote(lambda));
        return source.Provider.CreateQuery<T>(result);
    }
}
