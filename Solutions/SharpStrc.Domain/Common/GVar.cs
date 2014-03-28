namespace SharpStrc.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    public partial class GVar
    {
        private static readonly Dictionary<Type, IList<ComboListItem>> EnumComboList =
            new Dictionary<Type, IList<ComboListItem>>();

        private static readonly Dictionary<Type, Dictionary<int, string>> EnumValueDescDic =
            new Dictionary<Type, Dictionary<int, string>>();

        static GVar()
        {
            RegisterEnum<AvatarSize>();
            RegisterEnum<ImageWatermarkPosition>();
            RegisterEnum<FileType>();
        }

        private static void RegisterEnum<T>() where T : struct, IConvertible
        {
            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("T must a enumerated type.", "t");
            }

            IList<ComboListItem> lists = ConvertEnumToComboListItem<T>();
            Dictionary<int, string> dic = ConvertEnumToValMapDic<T>();

            EnumComboList.Add(typeof (T), lists);
            EnumValueDescDic.Add(typeof (T), dic);
        }

        private static IList<ComboListItem> ConvertEnumToComboListItem<T>() where T : struct, IConvertible
        {
            var listItems = new List<ComboListItem>();

            Type type = typeof (DescriptionAttribute);

            foreach (FieldInfo fi in typeof (T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                object[] arr = fi.GetCustomAttributes(type, true);
                if (arr.Length > 0)
                {
                    listItems.Add(new ComboListItem
                        {
                            Text = ((DescriptionAttribute) arr[0]).Description,
                            Value = fi.GetRawConstantValue().ToString()
                        });
                }
            }

            return listItems;
        }

        private static Dictionary<int, string> ConvertEnumToValMapDic<T>() where T : struct, IConvertible
        {
            var dic = new Dictionary<int, string>();

            Type type = typeof (DescriptionAttribute);

            foreach (FieldInfo fi in typeof (T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                object[] arr = fi.GetCustomAttributes(type, true);
                if (arr.Length > 0)
                {
                    dic.Add((int) fi.GetRawConstantValue(), ((DescriptionAttribute) arr[0]).Description);
                }
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
    }

    public class ComboListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}