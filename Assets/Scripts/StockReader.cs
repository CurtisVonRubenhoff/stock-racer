using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class StockReader : MonoBehaviour {
  private string url = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=DJIA&interval=15min&apikey=RRK89HCB2UIPLVHO";

  [SerializeField]
  private LineRenderer myLine;
  [SerializeField]

  private List<StockEntry> stocksList = new List<StockEntry> ();

  IEnumerator Start () {
    using (UnityWebRequest webRequest = UnityWebRequest.Get (url)) {
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
    Vector3[] points = new Vector3[100];
    for (var i = 99; i > -1; i--) {
      Debug.Log(lineData[i].datetime);
      points[i] = new Vector3 (i, float.Parse(lineData[i].close)/ 200f, 0);
    }

    myLine.SetPositions(points);
  }
}

public class StockEntry {
  public string datetime;
  public string open;
  public string high;
  public string low;
  public string close;
  public string volume;

  public StockEntry (string dt, string o, string h, string l, string c, string v) {
    datetime = dt;
    open = o;
    high = h;
    low = l;
    close = c;
    volume = v;
  }
}
