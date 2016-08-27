namespace Darc.Dapper.Expressions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class QueryExtension
    {
        public static string WhereC<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            var expression = Expression.Call(null, ((MethodInfo) MethodBase.GetCurrentMethod())
                .MakeGenericMethod(typeof (TSource)),
                new[] {source.Expression, Expression.Quote(predicate)});

            var translator = new QueryTranslator();
            return translator.Translate(expression);
        }
    }
}