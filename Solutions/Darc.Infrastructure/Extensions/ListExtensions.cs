namespace Darc.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> observableCollection, IEnumerable<T> collection)
        {
            if (observableCollection == null)
            {
                throw new ArgumentNullException(nameof(observableCollection));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            collection.Each(observableCollection.Add);
        }

        public static string Join(this IList<string> items, string delimeter)
        {
            string joined = "";
            int ndx;
            for (ndx = 0; ndx < items.Count - 2; ndx++)
            {
                joined += items[ndx] + delimeter;
            }
            joined += items[ndx];
            return joined;
        }

        public static string Join<T>(this IList<T> items, string delimeter)
        {
            if (items == null || items.Count == 0)
                return string.Empty;

            if (items.Count == 1)
                return items[0].ToString();

            var buffer = new StringBuilder();
            buffer.Append(items[0]);

            for (int ndx = 1; ndx < items.Count; ndx++)
            {
                string append = items[ndx].ToString();
                buffer.Append(delimeter + append);
            }
            return buffer.ToString();
        }

        public static string JoinDelimited<T>(this IList<T> items, string delimeter, Func<T, string> appender)
        {
            if (items == null || items.Count == 0)
                return string.Empty;

            if (items.Count == 1)
                return appender(items[0]);

            var buffer = new StringBuilder();
            string val = appender == null ? items[0].ToString() : appender(items[0]);
            buffer.Append(val);

            for (int ndx = 1; ndx < items.Count; ndx++)
            {
                T item = items[ndx];
                val = appender == null ? item.ToString() : appender(item);
                buffer.Append(delimeter + val);
            }
            return buffer.ToString();
        }

        public static string JoinDelimitedWithNewLine<T>(this IList<T> items, string delimeter, int newLineAfterCount,
                                                         string newLineText, Func<T, string> appender)
        {
            if (items == null || items.Count == 0)
                return string.Empty;

            if (items.Count == 1)
                return appender(items[0]);

            var buffer = new StringBuilder();
            buffer.Append(appender(items[0]));

            for (int ndx = 1; ndx < items.Count; ndx++)
            {
                T item = items[ndx];
                string append = appender(item);
                if (ndx%newLineAfterCount == 0)
                    buffer.Append(newLineText);

                buffer.Append(delimeter + append);
            }
            return buffer.ToString();
        }

        public static string ToHtml(this IList<string> items)
        {
            return items.Aggregate((x, y) => x + "<br />" + y);
        }
    }
}