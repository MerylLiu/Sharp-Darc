namespace Darc.Dapper.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using Attributes;

    internal class CommonUtil
    {
        public static string GetTableName<T>()
        {
            var ty = typeof (T);
            var arri = ty.GetCustomAttributes(typeof (BaseAttribute), true)
                .FirstOrDefault(p => p.GetType() == typeof (TableAttribute));
            if (arri is TableAttribute && (!string.IsNullOrEmpty((arri as BaseAttribute).Name)))
            {
                return (arri as BaseAttribute).Name;
            }
            return ty.Name;
        }

        public static string GetDefaultOrderField<T>()
        {
            var name = "";
            foreach (var propertyInfo in typeof (T).GetProperties())
            {
                var arri = propertyInfo.GetCustomAttributes(typeof (BaseAttribute), true)
                    .FirstOrDefault(p => p.GetType() == typeof (IgnoreAttribute));

                if (arri is IgnoreAttribute)
                {
                    arri = null;
                    continue;
                }
                name = (string.IsNullOrEmpty((arri as BaseAttribute)?.Name))
                    ? propertyInfo.Name
                    : (arri as BaseAttribute).Name;
                break;
            }
            return name;
        }

        public static IList<ParamColumnModel> GetExecutedColumns<T>() where T : class
        {
            var columns = new List<ParamColumnModel>();
            var properties = typeof (T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var arri = propertyInfo.GetCustomAttributes(typeof (BaseAttribute), true)
                    .FirstOrDefault(p => p.GetType() == typeof (ParamColumnModel));

                if (arri is IgnoreAttribute)
                {
                    arri = null;
                    continue;
                }
                if (arri is PrimaryKeyAttribute)
                {
                    if ((arri as PrimaryKeyAttribute).AutoIncrement)
                    {
                        arri = null;
                        continue;
                    }
                }
                /*else if (arri is ColumnAttribute)
                {
                    if ((arri as ColumnAttribute).AutoIncrement)
                    {
                        arri = null;
                        continue;
                    }
                }*/
                var name = (string.IsNullOrEmpty((arri as BaseAttribute)?.Name))
                    ? propertyInfo.Name
                    : (arri as BaseAttribute).Name;
                columns.Add(new ParamColumnModel
                {
                    ColumnName = name,
                    FieldName = propertyInfo.Name
                });
            }
            return columns;
        }

        public static IList<ParamColumnModel> GetExecutedColumns<T>(T t) where T : class
        {
            var columns = new List<ParamColumnModel>();
            var properties = typeof (T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var arri = propertyInfo.GetCustomAttributes(typeof (BaseAttribute), true)
                    .FirstOrDefault(p => p.GetType() == typeof (ParamColumnModel));

                if (arri is IgnoreAttribute)
                {
                    arri = null;
                    continue;
                }
                if (arri is PrimaryKeyAttribute)
                {
                    if ((arri as PrimaryKeyAttribute).AutoIncrement)
                    {
                        arri = null;
                        continue;
                    }
                }
                /*else if (arri is ColumnAttribute)
                {
                    if ((arri as ColumnAttribute).AutoIncrement)
                    {
                        arri = null;
                        continue;
                    }
                }*/
                var name = (string.IsNullOrEmpty((arri as BaseAttribute)?.Name))
                    ? propertyInfo.Name
                    : (arri as BaseAttribute).Name;
                columns.Add(new ParamColumnModel
                {
                    ColumnName = name,
                    FieldName = propertyInfo.Name,
                    FieldValue = propertyInfo.GetValue(t, null) == null
                        ? null
                        : propertyInfo.GetValue(t, null).ToString()
                });
            }
            return columns;
        }

        public static string GetPrimaryKey<T>() where T : class
        {
            var name = "";

            foreach (var propertyInfo in typeof (T).GetProperties())
            {
                var arri = propertyInfo.GetCustomAttributes(typeof (BaseAttribute), true)
                    .FirstOrDefault(p => p.GetType() == typeof (PrimaryKeyAttribute));

                if (arri is PrimaryKeyAttribute)
                {
                    name = string.IsNullOrEmpty((arri as BaseAttribute).Name)
                        ? propertyInfo.Name
                        : (arri as BaseAttribute).Name;
                    break;
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("The primary key is not exist");
            }

            return name;
        }

        public static string GetExecutedCloumName<T>(string propertyName) where T : class
        {
            var propertyInfo = typeof (T).GetProperty(propertyName);
            var arri = propertyInfo.GetCustomAttributes(typeof (BaseAttribute), true).FirstOrDefault();

            if (arri is IgnoreAttribute)
            {
                arri = null;
            }

            var name = (arri == null || string.IsNullOrEmpty((arri as BaseAttribute).Name))
                ? propertyInfo.Name
                : (arri as BaseAttribute).Name;

            return name;
        }

        public static string GetNameByExpress<T>(Expression<Func<T, object>> expr) where T : class
        {
            var pname = "";
            if (expr.Body is UnaryExpression)
            {
                var uy = expr.Body as UnaryExpression;
                pname = (uy.Operand as MemberExpression).Member.Name;
            }
            else
            {
                pname = (expr.Body as MemberExpression).Member.Name;
            }
            var propertyInfo = typeof (T).GetProperty(pname);
            var arri = propertyInfo.GetCustomAttributes(typeof (BaseAttribute), true).FirstOrDefault();

            if (arri is IgnoreAttribute)
                throw new Exception(string.Format("{0} can't be able to generate SQL", pname));

            var name = (arri == null || string.IsNullOrEmpty((arri as BaseAttribute).Name))
                ? propertyInfo.Name
                : (arri as BaseAttribute).Name;
            return name;
        }

        public static string GetSequence<T>()
        {
            var ty = typeof (T);
            var arri = ty.GetCustomAttributes(typeof (BaseAttribute), true)
                .FirstOrDefault(p => p.GetType() == typeof (SequenceAttribute));

            if (!string.IsNullOrEmpty((arri as SequenceAttribute)?.Sequence))
            {
                return (arri as SequenceAttribute).Sequence;
            }
            return ty.Name;
        }

        public static string UnitMoreSpace(string str)
        {
            var replaceSpace = new Regex(@"\s{1,}", RegexOptions.IgnoreCase);
            return replaceSpace.Replace(str, " ").Trim();
        }
    }
}