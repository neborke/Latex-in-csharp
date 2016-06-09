﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moosetrail.LaTeX
{
    /// <summary>
    /// Helperclass that holds handling of formating text in regular text
    /// </summary>
    internal static class TextFormatStrings
    {
        public static List<string> SingleFormat = new List<string>
        {
            "\n",
            "\r",
            "\t",
        };

        public static List<string> DoubleFormat = new List<string>
        {
            @"\\n",
            @"\\t",
            @"\\r"
        };

        public static string RemoveStartingFormating(string str)
        {
            var sb = new StringBuilder(str);
            return RemoveStartingFormating(sb).ToString();
        }

        public static StringBuilder RemoveStartingFormating(StringBuilder str)
        {
            var s = str.ToString();

            while (DoubleFormat.Any(x => str.ToString().StartsWith(x)) || str.ToString().StartsWith(" "))
            {
                str.Remove(0, str.ToString().StartsWith(" ") ? 1 : 2);
            }

            while (SingleFormat.Any(x => str.ToString().StartsWith(x)) || str.ToString().StartsWith(" "))
            {
                str.Remove(0, 1);
            }

            return str;
        }

        public static bool EmptyString(string str)
        {
            var stringParts = str.Split(SingleFormat.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            return stringParts.Length == 0;
        }
    }
}