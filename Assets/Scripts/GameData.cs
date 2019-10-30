#if DEBUG
#define NOSAVEg
#endif

#define OPMODE

using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Items
{
    public string name;
    public float cost; //How much will the next item cost
    public int count; //How many do we have
    public float inc; //How much will it increase our money
}

public class Upgrade
{
    public string name;
    public float cost; //How much will the next item cost
    public int count; //How many do we have
    public float BaseMult;
}

public class GameData : MonoBehaviour
{

    const string gameversion = "Alpha v0.0.3.3";
    const string buildtype =
#if UNITY_ANDROID || UNITY_IPHONE
        "Mobile"
#elif UNITY_WEBGL
        "Browser"
#elif UNITY_EDITOR
        "Editor"
#else
        "Standalone"
#endif
        + " " +
#if DEBUG
        "Development" +
#else
        "Release" + 
#endif
" Build";

    public Dictionary<string, Items> ItemsList; //Declare dictionary where name points to corresponding Items object
    public Dictionary<string, Upgrade> UpgradesList; //Declare dictionary where name points to corresponding Upgrade object

    public float MPC; //Money Per Click
    public float BaseMPC;

    float _money; //Money counter, start at 0 of course
    public float Money {
        get {
            return this._money; //Get internal money value
        }
        set {
            UpdateMoney(value); //Update new Money
        }
    }
    List<Text> MoneyText; //So we don't have to constantly find the GameObject when updating the UI

    float SaveInterval = 60f;

    void Start() {
        this.MoneyText = new List<Text>();
        ChangeCanvas("MainCanvas");
        Application.runInBackground = true;
        foreach (Transform obj in GameObject.Find("MainGameCanvases").transform) {
            Text t = obj.GetComponent<Canvas>().transform.Find("Money").GetComponent<Text>();
            if (t != null)
                this.MoneyText.Add(t);
            Text text = obj.GetComponent<Canvas>().transform.Find("InfoText").GetComponent<Text>();
            if (text != null)
                text.text = buildtype + " " + gameversion;
        }

        this.ItemsList = new Dictionary<string, Items>(); //Initialize dictionary
        this.UpgradesList = new Dictionary<string, Upgrade>(); //Initialize dictionary
#if !NOSAVE
        if (PlayerPrefs.HasKey("SaveInterval"))
            this.SaveInterval = PlayerPrefs.GetFloat("SaveInterval");
        foreach (Transform obj in GameObject.Find("Coders").transform) {
            Button btn = obj.GetComponentInChildren<Button>();
            Purchase p = obj.GetComponent<Purchase>();

            int Savedcount = 0;
            if (PlayerPrefs.HasKey(btn.name))
                Savedcount = PlayerPrefs.GetInt(btn.name);

            float price = p.BaseCost;
            if (Savedcount > 0)
                price = Calcbuycost(1, p.BaseCost, p.CoEff, Savedcount);

            Items tosave = new Items {
                cost = price,
                count = Savedcount,
                inc = p.BaseInc,
                name = btn.name
            };
            this.ItemsList.Add(btn.name, tosave);
        }
        foreach (Transform obj in GameObject.Find("UpgradesList").transform) {
            Button btn = obj.GetComponentInChildren<Button>();
            UpgradeBtn p = obj.GetComponent<UpgradeBtn>();

            int Savedcount = 0;
            if (PlayerPrefs.HasKey(btn.name))
                Savedcount = PlayerPrefs.GetInt(btn.name);

            float price = p.BaseCost;
            if (Savedcount > 0)
                price = Calcbuycost(1, p.BaseCost, p.CoEff, Savedcount);

            Upgrade tosave = new Upgrade {
                cost = price,
                count = Savedcount,
                BaseMult = p.MultGain,
                name = btn.name
            };
            this.UpgradesList.Add(btn.name, tosave);
        }
#endif

        this.MPC = 1f;
        this.BaseMPC = this.MPC;
#if !NOSAVE
        this._money = PlayerPrefs.HasKey("Money") ? PlayerPrefs.GetFloat("Money") : 0f;
        InvokeRepeating("Save", this.SaveInterval, this.SaveInterval);
#else
        this._money = 0f;
#endif
        StartCoroutine("MoneyUpdate");
    }
    const int smoothness = 30;
    System.Collections.IEnumerator MoneyUpdate() {
        for (; ; ) {
            if (this.ItemsList.Count > 0) { //If there are any items in the list
                foreach (Items i in this.ItemsList.Values) { //Go through all the different items
                    float toadd = i.inc * i.count; //If we have 2 Gainers which gives 1 each, we do 1 * 2 to get 2
                    UpdateMoney(this._money + toadd / smoothness); //Update new Money
                }
            }
            yield return new WaitForSeconds(1f / smoothness);
        }
    }
    void OnGUI() {
        GUI.Label(new Rect(10, 10, 100, 20), this.avg_fps.ToString("0") + " fps");
    }

    private float ListAvarage(List<float> l) {
        float total = 0;
        foreach(float part in l) {
            total += part;
        }
        return total / l.Count;
    }

    float deltaTime = 0.0f;
    List<float> fpses = new List<float>();
    float avg_fps = 0f;
    void Update() {
        this.deltaTime += Time.deltaTime;
        this.deltaTime /= 2f;
        this.fpses.Add(1f / this.deltaTime);
        if(this.fpses.Count > 100) {
            this.avg_fps = ListAvarage(this.fpses);
            this.fpses.Clear();
        }
#if UNITY_STANDALONE
        if (Input.GetKey(KeyCode.Escape)) {
            Debug.Log("Exitting");
            Application.Quit();
        }
#endif
    }
    [MethodImpl(256)]
    public void Click() {
        AddMoney(this.MPC);
    }
    [MethodImpl(256)]
    public void AddMoney(float val) {
        this.Money += val;
    }
    [MethodImpl(256)]
    public void RemoveMoney(float val) {
        this.Money -= val;
    }
    [MethodImpl(256)]
    public bool Canafford(float price) {
        return this.Money >= price;
    }

    public void UpdateMoney(float newval) {
        this._money = newval;
#if DEBUG
        if (this.MoneyText != null) {
            if (this.MoneyText.Count > 0) {
#endif
                for (int i = 0; i < this.MoneyText.Count; i++) {
                    this.MoneyText[i].text = "Money: " + FloatString(this.Money, "0.00"); //Round the Money float to x.xx to not make it huge
                }
#if DEBUG
            } else {
                Debug.Log("No money texts?");
            }
        } else {
            Debug.Log("No MoneyText list?");
        }
#endif
    }

    static string[] Suffixes =
    {
        "",
        "K",
        "M",
        "B",
        "T",
        "Q",
        "Infinity"
    };
    public static string FloatString(float val, string format = "") {
        string floatstr = val.ToString(format);
        string highnum = floatstr.Split('.')[0];

        float num = val;

        int count = 0;
        while (highnum.Length > 3) {
            if (count + 1 >= Suffixes.Length)
                break;
            count++;
            num /= 1000;
            floatstr = num.ToString(format);
            highnum = floatstr.Split('.')[0];

            if (highnum.Length > 2) {
                floatstr = highnum;
            }
        }
        return floatstr + Suffixes[count];
    }
    public void Save() {
#if !NOSAVE
        if (this.ItemsList == null)
            return;
        PlayerPrefs.SetFloat("Money", this.Money);
        foreach (Items i in this.ItemsList.Values) {
            PlayerPrefs.SetInt(i.name, i.count);
        }
        foreach (Upgrade i in this.UpgradesList.Values) {
            PlayerPrefs.SetInt(i.name, i.count);
        }
        PlayerPrefs.Save();
#if DEBUG
        Debug.Log("Saving Finished!");
#endif
#endif
    }
    void OnApplicationQuit() {
        Save();
    }
    void OnApplicationPause(bool pausing) {
        if (pausing)
            Save();
    }
    void OnApplicationFocus(bool hasFocus) {
        Save();
    }

    [MethodImpl(256)]
    public static bool ChangeCanvas(string newcanvas) {
        GameObject canvases = GameObject.Find("Canvases");
        foreach (Canvas c in canvases.GetComponentsInChildren<Canvas>()) {
            c.enabled = false;
        }
        GameObject obj = GameObject.Find(newcanvas);
#if DEBUG
        if (obj == null) {
            Debug.Log("Not a canvas! Did you type the correct name?");
            return false;
        }
#endif
        Canvas canvas = obj.GetComponent<Canvas>();
#if DEBUG
        if (canvas == null) {
            Debug.Log("Not a canvas! Did you type the correct name?");
            return false;
        }
#endif
        canvas.enabled = true;
        return true;
    }

    public void ChangeSaveInterval(float newtime) {
#if !NOSAVE
        CancelInvoke("Save");
        InvokeRepeating("Save", newtime, newtime);
        this.SaveInterval = newtime;
        PlayerPrefs.SetFloat("SaveInterval", newtime);
#if DEBUG
        Debug.Log("Invoking Save every " + newtime + " seconds");
#endif
#endif
    }
    public static float Calcbuycost(float amounttobuy, float basePrice, float coEff, float CurrOwn) {
        return
        basePrice *
        (
            (
              Mathf.Pow(coEff, (CurrOwn + 1)) *
              (Mathf.Pow(coEff, amounttobuy) - 1) //Mathf.Pow(CoEff, amounttobuy - 1)?
            ) / (
              coEff - 1
            )
        );
    }
    public void Reset() {
        this._money = 0;
        foreach (Transform obj in GameObject.Find("Coders").transform) {
            var t = obj.GetComponent<Purchase>();

            Items i;
            this.ItemsList.TryGetValue(t.name, out i);

            i.count = 0;
            i.cost = t.BaseCost;

            t.UpdateLabel();
        }
        foreach (Transform obj in GameObject.Find("UpgradesList").transform) {
            var t = obj.GetComponent<UpgradeBtn>();

            Upgrade i;
            this.UpgradesList.TryGetValue(t.name, out i);

            i.count = 0;
            i.cost = t.BaseCost;

            t.UpdateLabel();
        }
    }
}
