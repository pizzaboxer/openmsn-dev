﻿using System.Text;

namespace OpenMSN.MSNPServer.Extensions
{
    public static class StringEx
    {
        private static IReadOnlyDictionary<char, string> s_KnownControls = new Dictionary<char, string>() 
        {
            { '\0', "\\0"},
            { '\a', "\\a"},
            { '\b', "\\b"},
            { '\n', "\\n"},
            { '\v', "\\v"},
            { '\t', "\\t"},
            { '\f', "\\f"},
            { '\r', "\\r"}
        };

        public static string ToLiteral(this string input)
        {
            if (input is null)
                return null;

            StringBuilder sb = new();

            foreach (var c in input)
            {
                if (char.IsControl(c))
                {
                    sb.Append(s_KnownControls.TryGetValue(c, out var s) ? s : $"\\u{((int)c):x4}");
                }
                else
                {
                    if (c == '"' || c == '\\') // escapement 
                        sb.Append('\\');

                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
