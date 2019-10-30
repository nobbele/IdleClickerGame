using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteAllBtn : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(OnClick);
	}

    void OnClick() {
        GameData gameData = GameObject.Find("GameData").GetComponent<GameData>();

        gameData.Reset();
        gameData.Save();
    }
}
