namespace SharpStrc.Framework.Utilities
{
    using System.Text.RegularExpressions;

    public static class RmbUtil
    {
        public static string ToUpperAmount(this decimal amount)
        {
            string s = amount.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s,
                                     @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))",
                                     "${b}${z}");
            string result = Regex.Replace(d, ".",
                                          delegate(Match m)
                                              {
                                                  return @"负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString();
                                              });

            if (amount.ToString().LastIndexOf('.') <= 0)
            {
                result += "整";
            }

            return result;
        }
    }
}