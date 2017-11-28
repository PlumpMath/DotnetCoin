using System.Linq;
using System.Text;

namespace Dotcoin
{
    public static class ExtensionMethods
    {
        public static byte[] ToByteArray(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string ToNormString(this byte[] bytes)
        {
            int[] bytesAsInts = bytes.Select(x => (int) x).ToArray();
            return string.Join("", bytesAsInts);
        }

        public static long ToLongNumber(this byte[] bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var b in bytes)
            {
                stringBuilder.Append((int) b);
            }
            return long.Parse(stringBuilder.ToString());
        }
    }
}