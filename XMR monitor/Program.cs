using Newtonsoft.Json;
using System.Diagnostics;

namespace XMR_monitor
{
  class Program
  {
    static void Main(string[] args)
    {





      Debug.WriteLine(JsonConvert.SerializeObject(HashParser.Parse()));

    }
  }
}