using HtmlAgilityPack;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace XMR_monitor
{
  public class HashData
  {
    public enum ProcessorType
    {
      Unknown,
      CPU,
      GPU,
    }

    public ProcessorType processorType;
    public int threadId;
    public float hash10s;
    public float hash60s;
    public float hash15m;

    public HashData(int threadId, float hash10s, float hash60s, float hash15m)
    {
      this.threadId = threadId;
      this.hash10s = hash10s;
      this.hash60s = hash60s;
      this.hash15m = hash15m;

      if (hash10s == 0)
      {
        processorType = ProcessorType.Unknown;
      }
      else if (hash10s < 200f)
      {
        processorType = ProcessorType.CPU;
      }
      else
      {
        processorType = ProcessorType.GPU;
      }
    }
  }

  public static class HashParser
  {
    public static readonly string url = "http://127.0.0.1:16000/h";

    public static HashData[] Parse()
    {
      // get data from web
      var htmlWeb = new HtmlWeb();

      HtmlDocument htmlDoc = htmlWeb.Load(url);

      var node = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'data')]/table");

      List<HashData> hashs = new List<HashData>();

      foreach (var tr in node.ChildNodes)
      {        
        if (!int.TryParse(tr.ChildNodes[0].InnerText, out int threadId))
        {
          continue;
        }

        float.TryParse(tr.ChildNodes[1].InnerText, out float hash10s);
        float.TryParse(tr.ChildNodes[2].InnerText, out float hash60s);
        float.TryParse(tr.ChildNodes[3].InnerText, out float hash15m);

        HashData hashdata = new HashData(threadId, hash10s, hash60s, hash15m);

        if (hashdata.processorType == HashData.ProcessorType.GPU)
        {
          hashs.Add(hashdata);
        }        
      }    

      return hashs.ToArray();    
    }

    static Timer timer;

    static void StartTimer()
    {
      timer = new Timer(1000);

      timer.AutoReset = true;

      timer.Elapsed += new ElapsedEventHandler(t_Elapsed);

      timer.Start();
    }

    static void EndTimer()
    {
      timer.Stop();
      timer = null;
    }

    private static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      foreach (HashData hashdata in Parse())
      {
        // main thread
        if (hashdata.threadId % 2 == 0)
        {
          if (hashdata.hash10s < 2000f)
          {
            // running below threshold
            Debug.Write("running below threshold:" + hashdata.hash10s);
          }
        }
        else
        {
          if (hashdata.hash10s < 1800f)
          {
            // running below threshold
            Debug.Write("running below threshold:" + hashdata.hash10s);
          }
        }
      }
    }    
  }
}
