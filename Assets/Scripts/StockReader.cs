using System.Collections;
using System.Linq;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class StockReader : MonoBehaviour {
  private string url = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={0}&interval=15min&apikey=RRK89HCB2UIPLVHO";

  public string stockName;

  [SerializeField]
  private LineRenderer myLine;
  [SerializeField]

  private List<StockEntry> stocksList = new List<StockEntry> ();

  IEnumerator Start () {
    var reqURL = string.Format(url, stockName);
    using (UnityWebRequest webRequest = UnityWebRequest.Get (reqURL)) {
      yield return webRequest.SendWebRequest ();

      JsonData respObj = JsonMapper.ToObject (webRequest.downloadHandler.text);
      var seriesData = respObj["Time Series (15min)"];
      var dataKeys = seriesData.Keys;

      foreach (var key in dataKeys) {
        var entry = seriesData[key];

        stocksList.Add (new StockEntry (
          key,
          entry["1. open"].ToString (),
          entry["2. high"].ToString (),
          entry["3. low"].ToString (),
          entry["4. close"].ToString (),
          entry["5. volume"].ToString ()
        ));
      }

      SetupLine (stocksList);

    }

    Debug.Log (stocksList);
  }

  private void SetupLine (List<StockEntry> lineData) {
    var max = lineData.Max((ld) => ld.low);
    var min = lineData.Min((ld) => ld.low);

    var range = max - min;

    Vector3[] points = new Vector3[100];
    for (var i = 99; i > -1; i--) {
      var val = (lineData[i].low - min) / range;
      Debug.Log(lineData[i].datetime);
      points[i] = new Vector3 (i, val * 100, 0);
    }

    myLine.SetPositions(points);
  }
}

public class StockEntry {
  public string datetime;
  public float open;
  public float high;
  public float low;
  public float close;
  public float volume;

  public StockEntry (string dt, string o, string h, string l, string c, string v) {
    datetime = dt;
    open = float.Parse(o);
    high = float.Parse(h);
    low = float.Parse(l);
    close = float.Parse(c);
    volume = float.Parse(v);
  }
}
