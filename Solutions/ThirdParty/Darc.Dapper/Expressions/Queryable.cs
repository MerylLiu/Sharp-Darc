namespace Darc.Dapper.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class Queryable<T> : IQueryable<T>
    {
        public Queryable()
        {
            Expression = Expression.Constant(this);
            Provider = new QueryProvider();
        }

        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<T>>(Expression)).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }

        public Type ElementType => typeof (T);
    }
}