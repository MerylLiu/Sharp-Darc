namespace Darc.Framework.Utilities
{
    using System.IO;
    using System.Xml.Serialization;

    public class XmlUtil
    {
        public static void Serialize<T>(string filename, T obj)
        {
            var xs = new XmlSerializer(typeof (T));
            using (var wr = new StreamWriter(filename))
            {
                xs.Serialize(wr, obj);
            }
        }

        public static T Deserialize<T>(string filename)
        {
            var xs = new XmlSerializer(typeof (T));
            using (var rd = new StreamReader(filename))
            {
                return (T) xs.Deserialize(rd);
            }
        }
    }
}