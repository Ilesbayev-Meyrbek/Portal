using System.Linq.Expressions;

namespace Portal.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, Expression<Func<T, object>>? orderByProperty, bool orderByDescending)
        {
            var selectorBody = orderByProperty.Body;
            // Strip the Convert expression
            if (selectorBody.NodeType == ExpressionType.Convert)
                selectorBody = ((UnaryExpression) selectorBody).Operand;
            // Create dynamic lambda expression
            var selector = Expression.Lambda(selectorBody, orderByProperty.Parameters);
            // Generate the corresponding Queryable method call
            var queryBody = Expression.Call(typeof(Queryable),
                orderByDescending ? "OrderByDescending" : "OrderBy",
                new Type[] {typeof(T), selectorBody.Type},
                source.Expression, Expression.Quote(selector));
            return source.Provider.CreateQuery<T>(queryBody);
        }
    }
}
