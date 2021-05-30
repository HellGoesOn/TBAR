using System;
using System.Text.RegularExpressions;

namespace TBAR.Helpers
{
    public static class StringExtension
    {
        public static string SpliceText(this string st, int lineLength)
        {
            return Regex.Replace(st, "(.{" + lineLength + "})" + ' ', "$1" + Environment.NewLine);
        }
    }
}
