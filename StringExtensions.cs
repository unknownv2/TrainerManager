
using System;
using System.Collections.Generic;
using System.Linq;


namespace TrainerManager;

public static class StringExtensions
{
  public static string ToPascalCase(this string str)
  {
    return ((IEnumerable<string>) str.Split(new string[1]
    {
      "_"
    }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (s => char.ToUpperInvariant(s[0]).ToString() + s.Substring(1, s.Length - 1))).Aggregate<string, string>(string.Empty, (Func<string, string, string>) ((s1, s2) => s1 + s2));
  }
}
