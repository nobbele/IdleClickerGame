using UnityEngine;
using UnityEngine.UI;

public class ManuallySaveBtn : MonoBehaviour {

    GameData gameData;

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(OnClick);
        this.gameData = GameObject.Find("GameData").GetComponent<GameData>();
	}

    void OnClick() {
        this.gameData.Save();
    }
}
