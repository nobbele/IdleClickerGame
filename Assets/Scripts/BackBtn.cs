using UnityEngine;
using UnityEngine.UI;

public class BackBtn : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(OnClick);
	}

    void OnClick() {
        GameData.ChangeCanvas("MainCanvas");
    }
}