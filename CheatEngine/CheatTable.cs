
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;


namespace TrainerManager.CheatEngine;

public class CheatTable
{
  [JsonProperty("@CheatEngineTableVersion")]
  public int Version { get; set; }

  public CheatEntries CheatEntries { get; set; }

  public CheatCodes CheatCodes { get; set; }

  public object UserdefinedSymbols { get; set; }

  public static CheatTable FromFile(string filename)
  {
    string xml = CheatTable.FixTags(File.ReadAllText(filename, Encoding.UTF8), (IEnumerable<string>) new string[6]
    {
      "Key",
      "Offset",
      "Hotkey",
      "CheatEntry",
      "CodeEntry",
      "Byte"
    });
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.LoadXml(xml);
    return JsonConvert.DeserializeObject<CheatTable>(JsonConvert.SerializeXmlNode(xmlDocument.GetElementsByTagName(nameof (CheatTable))[0], Newtonsoft.Json.Formatting.None, true));
  }

  private static string FixTags(string xml, IEnumerable<string> tags)
  {
    xml = xml.Replace("<CheatTable ", "<CheatTable xmlns:json='http://james.newtonking.com/projects/json' ");
    foreach (string tag in tags)
      xml = xml.Replace($"<{tag}>", $"<{tag} json:Array='true'>");
    return xml;
  }
}
