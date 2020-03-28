using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class StockReader : MonoBehaviour {
  private string url = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={0}&interval=15min&apikey=RRK89HCB2UIPLVHO";

  public string stockName;

  [SerializeField]
  private LineRenderer myLine;
  [SerializeField]
  private EdgeCollider2D myEdge;
  [SerializeField]
  private TextMeshProUGUI myTmp;

  [SerializeField]
  private GameObject TextInput;
  [SerializeField]
  private GameObject PlayerObject;
  [SerializeField]
  private GameObject InstructionsObject;

  private List<StockEntry> stocksList = new List<StockEntry> ();

  private bool gameRunning = false;

  void Update () {
    if (Input.GetKeyDown (KeyCode.Backspace)) {
      ResetGame ();
    }

  }

  public void updateStockName (string value) {
    Debug.Log (value.ToUpper ());
    stockName = value.ToUpper ();
    InstructionsObject.SetActive (false);
    StartCoroutine (getLineData ());
    TextInput.SetActive (false);
  }

  public IEnumerator getLineData () {
    var reqURL = string.Format (url, stockName);
    using (UnityWebRequest webRequest = UnityWebRequest.Get (reqURL)) {
      yield return webRequest.SendWebRequest ();
      myTmp.text = stockName;
      PlayerObject.SetActive (true);
      var text = webRequest.downloadHandler.text;
      JsonData respObj = JsonMapper.ToObject (text);
      HandleServerResponse (respObj);
    }
  }

  private void HandleServerResponse (JsonData resp) {

    try {
      var seriesData = resp["Time Series (15min)"];
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

    } catch (System.Exception e) {
      myTmp.text = "Invalid Symbol";
    }

    gameRunning = true;

    stocksList.Clear ();
  }

  public void ResetGame () {
    gameRunning = false;
    PlayerObject.SetActive (false);
    PlayerObject.transform.localScale = new Vector3(8, 8, 1);
    PlayerObject.GetComponent<PlayerController> ().isControlled = true;
    myTmp.text = "";
    TextInput.GetComponent<TMP_InputField> ().text = "";
    TextInput.SetActive (true);
    InstructionsObject.SetActive (true);
    myLine.positionCount = 0;
  }

  private void SetupLine (List<StockEntry> lineData) {
    myLine.positionCount = lineData.Count;

    var max = lineData.Max ((ld) => ld.low);
    var min = lineData.Min ((ld) => ld.low);

    var range = max - min;

    Vector3[] points = new Vector3[lineData.Count];
    Vector2[] edgePoints = new Vector2[lineData.Count];
    for (var i = lineData.Count - 1; i > -1; i--) {
      var val = (lineData[i].low - min) / range;
      points[i] = new Vector3 (((float) i / (float) lineData.Count) * 150f, val * 100, 0);
      edgePoints[i] = new Vector2 (((float) i / (float) lineData.Count) * 150f, val * 100);
    }

    myLine.SetPositions (points);
    myEdge.points = edgePoints;
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
    open = float.Parse (o);
    high = float.Parse (h);
    low = float.Parse (l);
    close = float.Parse (c);
    volume = float.Parse (v);
  }
}
