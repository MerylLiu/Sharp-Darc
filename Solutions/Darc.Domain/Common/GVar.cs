namespace Darc.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public partial class GVar
    {
        private static readonly Dictionary<Type, IList<ComboListItem>> EnumComboList =
            new Dictionary<Type, IList<ComboListItem>>();

        private static readonly Dictionary<Type, Dictionary<int, string>> EnumValueDescDic =
            new Dictionary<Type, Dictionary<int, string>>();

               static GVar()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            types.Where(p => p.IsEnum).ToList().ForEach(RegisterEnum);
        }

        private static void RegisterEnum<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must a enumerated type.", "t");

            var lists = ConvertEnumToComboListItem<T>();
            var dic = ConvertEnumToValMapDic<T>();

            EnumComboList.Add(typeof(T), lists);
            EnumValueDescDic.Add(typeof(T), dic);
        }

        private static void RegisterEnum(Type type)
        {
            if (!type.IsEnum) throw new ArgumentException("T must a enumerated type.", "t");

            var lists = ConvertEnumToComboListItem(type);
            var dic = ConvertEnumToValMapDic(type);

            EnumComboList.Add(type, lists);
            EnumValueDescDic.Add(type, dic);
        }

        private static IList<ComboListItem> ConvertEnumToComboListItem<T>() where T : struct, IConvertible
        {
            var type = typeof(DescriptionAttribute);

            var listItems = ConvertEnumToComboListItem(type);
            return listItems;
        }

        private static IList<ComboListItem> ConvertEnumToComboListItem(Type type)
        {
            var listItems = new List<ComboListItem>();

            var desc = typeof(DescriptionAttribute);

            foreach (var fi in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var arr = fi.GetCustomAttributes(desc, true);
                if (arr.Length > 0)
                    listItems.Add(new ComboListItem
                    {
                        Text = ((DescriptionAttribute) arr[0]).Description,
                        Value = fi.GetRawConstantValue().ToString()
                    });
            }

            return listItems;
        }

        private static Dictionary<int, string> ConvertEnumToValMapDic<T>() where T : struct, IConvertible
        {
            var type = typeof(DescriptionAttribute);

            var dic = ConvertEnumToValMapDic(type);
            return dic;
        }

        private static Dictionary<int, string> ConvertEnumToValMapDic(Type type)
        {
            var dic = new Dictionary<int, string>();

            var desc = typeof(DescriptionAttribute);

            foreach (var fi in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var arr = fi.GetCustomAttributes(desc, true);
                if (arr.Length > 0)
                    dic.Add((int) fi.GetRawConstantValue(), ((DescriptionAttribute) arr[0]).Description);
            }

            return dic;
        }

        public static string GetEnumValDescription<T>(int value) where T : struct, IConvertible
        {
            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("T must a enumerated type.");
            }

            Type type = typeof (T);
            Dictionary<int, string> mapDic = EnumValueDescDic[type];

            return mapDic[value];
        }

        public static Dictionary<string, string> GetEnumMapDic<T>() where T : struct, IConvertible
        {
            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("T must a enumerated type.");
            }

            Type type = typeof (T);
            Dictionary<int, string> mapDic = EnumValueDescDic[type];

            var result = new Dictionary<string, string>();
            foreach (var pair in mapDic)
            {
                result.Add(pair.Key.ToString(), pair.Value);
            }

            return result;
        }

        public static IEnumerable<ComboListItem> GetComboListItems<T>() where T : struct, IConvertible
        {
            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("T must a enumerated type.");
            }

            Type type = typeof (T);

            return EnumComboList[type];
        }

        public static IList<ComboListItem> GetBoolComboListItems(string yes, string no)
        {
            var listItems = new List<ComboListItem>
                {
                    new ComboListItem
                        {
                            Text = yes,
                            Value = "true"
                        },
                    new ComboListItem
                        {
                            Text = no,
                            Value = "false"
                        }
                };

            return listItems;
        }
        
        public static T GetByIndex<T>(this IEnumerable<T> list, int index)
        {
            if (list == null || index >= list.Count() || index < 0) return default(T);
            return list.ToArray()[index];
        }

        public static T GetEnumByDesc<T>(string description) where T : Enum
        {
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs.Length > 0 && (objs[0] as DescriptionAttribute)?.Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException($"{description} 未能找到对应的枚举.", "Description");
        }

        public static T GetEnumByName<T>(string name) where T : Enum
        {
            var res = (T) Enum.Parse(typeof(T), name);
            return res;
        }

        public static T GetEnumByValue<T>(object value) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));

            foreach (var item in values)
            {
                if (Convert.ToInt32(value).Equals(Convert.ToInt32(item))) return (T) item;
            }

            return default(T);
        }
    }

    public class ComboListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
