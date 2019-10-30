using UnityEngine;
using UnityEngine.UI;

public class ClickerButton : MonoBehaviour {

    GameData gameData;

	// Use this for initialization
	void Start () {
        this.gameData = GameObject.Find("GameData").GetComponent<GameData>();
        Button btn = this.GetComponent<Button>();


        btn.onClick.AddListener(call: this.gameData.Click);
    }
}
