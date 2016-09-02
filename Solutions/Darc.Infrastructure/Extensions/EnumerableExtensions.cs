namespace Darc.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    public static class EnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            foreach (T local in items)
            {
                action(local);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
                action(item);
        }

        public static Boolean IsEmpty<T>(this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            bool isEmpty = !items.GetEnumerator().MoveNext();
            /* Reset the enumerator back to the starting position in the off
             * chance that we have a very poorly implemented IEnumerable
             * that does not return a *new* enumerator with every invocation
             * of the GetEnumerator method. */
            try
            {
                items.GetEnumerator().Reset();
            }
                // If this method is not supported, just skip the operation
            catch (NotSupportedException)
            {
            }

            return isEmpty;
        }

        public static string AsDelimited<T>(this IEnumerable<T> items, string delimeter)
        {
            var itemList = new List<string>();
            foreach (T item in items)
            {
                itemList.Add(item.ToString());
            }
            return String.Join(delimeter, itemList.ToArray());
        }

        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || items.IsEmpty();
        }

        public static bool HasAnyNulls<T>(this IEnumerable<T> items)
        {
            return IsTrueForAny(items, t => t == null);
        }

        public static bool IsTrueForAny<T>(this IEnumerable<T> items, Func<T, bool> executor)
        {
            foreach (T item in items)
            {
                bool result = executor(item);
                if (result)
                    return true;
            }
            return false;
        }

        public static bool IsTrueForAll<T>(this IEnumerable<T> items, Func<T, bool> executor)
        {
            foreach (T item in items)
            {
                bool result = executor(item);
                if (!result)
                    return false;
            }
            return true;
        }

        public static IDictionary<T, T> ToDictionary<T>(this IList<T> items)
        {
            IDictionary<T, T> dict = new Dictionary<T, T>();
            foreach (T item in items)
            {
                dict[item] = item;
            }
            return dict;
        }

        public static Dictionary<string, object> ToDictionary(this object value)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (PropertyInfo info in value.GetType().GetProperties())
            {
                try
                {
                    dictionary.Add(info.Name, info.GetValue(value, null));
                }
                catch
                {
                }
            }
            return dictionary;
        }

        public static IEnumerable<T> ReplaceItem<T>(this IEnumerable<T> items, Func<T,T> action)
        {
            var replaceItem = items as IList<T> ?? items.ToList();

            for (int i = 0; i < replaceItem.Count; i++)
            {
                replaceItem[i] = action(replaceItem[i]);
            }

            return replaceItem;
        }
    }
}