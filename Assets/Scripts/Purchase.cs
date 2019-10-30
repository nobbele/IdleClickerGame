#define OPMODE

using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class Purchase : MonoBehaviour {

    GameData gameData;
    Text Label;

    [HideInInspector]
    public string type = "Generic";

    [SerializeField]
    public float BaseCost = 10f, CoEff = 1f, BaseInc = 1f;

    Items items = new Items();

    // Use this for initialization
    void Start() {

        //Initialization
        Button btn = this.GetComponent<Button>();
        this.Label = this.transform.Find("Info").GetComponent<Text>();
        Text ButtonText = this.transform.Find("ButtonText").GetComponent<Text>();
        this.gameData = GameObject.Find("GameData").GetComponent<GameData>();
        this.type = this.name;

        ButtonText.text = "Hire " + this.type;

        btn.onClick.AddListener(OnClick); //Add a listener to the button so when the user click the button, OnClick() gets executed
        if (this.gameData.ItemsList.ContainsKey(this.type))
            this.items = this.gameData.ItemsList[this.type];
        else {
#if OPMODE
            this.BaseInc *= 10;
            this.BaseCost = 0;
#endif
            this.items.inc = this.BaseInc;
            this.items.name = this.type;
            this.items.cost = this.BaseCost;
            this.gameData.ItemsList[this.type] = this.items;
        }
        UpdateLabel();
    }

    void OnClick() {
        if(this.gameData.Canafford(this.items.cost)) {
            this.gameData.RemoveMoney(this.items.cost);
            Buy();
        }
    }

    void Buy() {
        int count = this.items.count + 1; //Increase count by 1
        float newcost = GameData.Calcbuycost(1, this.BaseCost, this.CoEff, this.items.count); //Sets new cost to cost calculation

        UpdateItems(count, newcost); //Set new cost and count
    }

    public void UpdateLabel() { //Update label with data
        StringBuilder sb = new StringBuilder();

        string cost = GameData.FloatString(this.items.cost, "0.00");
        sb.AppendLine(System.String.Format("It costs {0} to hire a {1}", cost, this.type));

        string CountOrNo = (this.items.count > 0 ? this.items.count.ToString() : "no");
        bool ShouldHaveSuffix = this.items.count > 1 || this.items.count == 0;
        string Suffix = (ShouldHaveSuffix ? "s" : "");
        sb.AppendLine(System.String.Format("You have {0} {1}{2} hired at the moment", CountOrNo, this.type, Suffix));

        float MPS = this.items.inc * this.items.count;
        sb.AppendLine(System.String.Format("Current Gain: {0} Money Per Second", MPS));

        this.Label.text = sb.ToString();
    }

    
    void UpdateItems(int count, float cost) {
        this.items.count = count; //Set the count to new count
        this.items.cost = cost; //Set itemscopy cost to the newcost

        this.gameData.ItemsList[this.type] = this.items; //Set real items in the itemslist to our items copy

        UpdateLabel(); //Update the label with new data
    }
}


