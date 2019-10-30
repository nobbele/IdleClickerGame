using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBtn : MonoBehaviour {

    GameData gameData;
    Text Label;

    [HideInInspector]
    public string type = "Generic";

    [SerializeField]
    public float BaseCost = 10f, CoEff = 1f, MultGain = 1f;

    Upgrade items = new Upgrade();

    // Use this for initialization
    void Start() {

        //Initialization
        Button btn = this.GetComponent<Button>();
        Text ButtonText = this.transform.Find("Text").GetComponent<Text>();
        this.Label = this.transform.Find("Info").GetComponent<Text>();
        this.gameData = GameObject.Find("GameData").GetComponent<GameData>();
        this.type = this.name;

        ButtonText.text = "Upgrade your " + this.type;

        btn.onClick.AddListener(OnClick); //Add a listener to the button so when the user click the button, OnClick() gets executed
        if (this.gameData.UpgradesList.ContainsKey(this.type))
            this.items = this.gameData.UpgradesList[this.type];
        else {
            this.items.name = this.type;
            this.items.cost = this.BaseCost;
            this.items.BaseMult = this.MultGain;
            this.gameData.UpgradesList[this.type] = this.items;
        }
        UpdateLabel();
    }

    void OnClick() {
        if (this.gameData.Canafford(this.items.cost)) {
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
        sb.AppendLine(System.String.Format("It costs {0} to upgrade your {1}", cost, this.type));

        sb.AppendLine(System.String.Format("You Own {0} {1}", this.items.count, this.type));

        float gain = (1 + (this.items.BaseMult * this.items.count));
        string gainstr = GameData.FloatString(gain, "0.00");
        sb.AppendLine(System.String.Format("Current gain is {0}x", gainstr));

        this.Label.text = sb.ToString();
    }
    void UpdateItems(int count, float cost) {
        this.items.count = count; //Set the count to new count
        this.items.cost = cost; //Set itemscopy cost to the newcost

        this.gameData.UpgradesList[this.type] = this.items; //Set real items in the UpgradesList to our items copy

        if (this.gameData.UpgradesList != null) {
            float TotalMultiplier = 0;
            if (this.gameData.UpgradesList.Count > 0) { //If there are any items in the list
                foreach (Upgrade i in this.gameData.UpgradesList.Values) { //Go through all the different items
                    float Multiplier = i.BaseMult * i.count;
                    TotalMultiplier += Multiplier;
                }
            }
            this.gameData.MPC = this.gameData.BaseMPC * (1 + TotalMultiplier);
        }

        UpdateLabel(); //Update the label with new data
    }
}
