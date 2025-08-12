
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;


namespace TrainerManager;

internal static class KeyConvert
{
  private static readonly System.Type KeyType = typeof (InputKey);
  public static readonly int[] F1 = new int[1]
  {
    112 /*0x70*/
  };

  internal static int[] FromKeyboardEvent(object e, MouseButtons mouseButtons)
  {
    List<InputKey> source = new List<InputKey>();
    return (int[])null;
  }

    internal static string ToString(int[] hotkey)
  {
    if (hotkey == null)
      return "";
    List<string> values = new List<string>(4);
    foreach (InputKey inputKey in ((IEnumerable<int>) hotkey).Where<int>((Func<int, bool>) (k => k != 0)).Cast<InputKey>())
    {
      MemberInfo[] member = KeyConvert.KeyType.GetMember(inputKey.ToString());
      if (member.Length != 0)
        values.Add(member[0].GetCustomAttributes<DescriptionAttribute>().First<DescriptionAttribute>().Description);
      else
        values.Add("INVALID");
    }
    return string.Join(" + ", (IEnumerable<string>) values);
  }
}
