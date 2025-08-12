
using System.IO;


namespace TrainerManager;

internal static class RandomString
{
  public static string Make() => Path.GetRandomFileName().Replace(".", "").Substring(0, 8);
}
