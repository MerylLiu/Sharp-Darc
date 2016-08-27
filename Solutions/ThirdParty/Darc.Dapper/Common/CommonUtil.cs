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
            var arri = ty.GetCustomAttributes(typeof (BaseAttribute), true).FirstOrDefault();
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
                var arri = propertyInfo.GetCustomAttributes(typeof (BaseAttribute), true).FirstOrDefault();
                if (arri is IgnoreAttribute)
                {
                    arri = null;
                    continue;
                }
                name = (arri == null || string.IsNullOrEmpty((arri as BaseAttribute).Name))
                    ? propertyInfo.Name
                    : (arri as BaseAttribute).Name;
                break;
            }
            return name;
        }

        public static IList<ParamColumnModel> GetExecutedColumns<T>() where T : class
        {
            var columns = new List<ParamColumnModel>();
            foreach (var propertyInfo in typeof (T).GetProperties())
            {
                var arri = propertyInfo.GetCustomAttributes(typeof (BaseAttribute), true).FirstOrDefault();
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
                var name = (arri == null || string.IsNullOrEmpty((arri as BaseAttribute).Name))
                    ? propertyInfo.Name
                    : (arri as BaseAttribute).Name;
                columns.Add(new ParamColumnModel {ColumnName = name, FieldName = propertyInfo.Name});
            }
            return columns;
        }

        public static string GetPrimaryKey<T>() where T : class
        {
            var name = "";

            foreach (var propertyInfo in typeof (T).GetProperties())
            {
                var arri = propertyInfo.GetCustomAttributes(typeof (BaseAttribute), true).FirstOrDefault();
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

        public static string UnitMoreSpace(string str)
        {
            var replaceSpace = new Regex(@"\s{1,}", RegexOptions.IgnoreCase);
            return replaceSpace.Replace(str, " ").Trim();
        }
    }
}