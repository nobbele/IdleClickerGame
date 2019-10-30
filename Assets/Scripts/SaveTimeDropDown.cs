using UnityEngine;
using UnityEngine.UI;

/* 
    * Creation date is
    * 18th of febuary 2018
*/

public class SaveTimeDropDown : MonoBehaviour {

    Dropdown dropdown;
    GameData gameData;

	// Use this for initialization
	void Start () {
        this.dropdown = GetComponent<Dropdown>();
        this.gameData = GameObject.Find("GameData").GetComponent<GameData>();

        this.dropdown.onValueChanged.AddListener(OnValueChanged);
	}

    void OnValueChanged(int newval) {
        string newoption = this.dropdown.options[newval].text;
        Debug.Log("Changing autosave time to " + newoption);
        string[] newoptionsplit = newoption.Split(' ');

        int amount = int.Parse(newoptionsplit[0]);

        string timetype = newoptionsplit[1];
        Debug.Log(timetype);

        int seconds = 60;
        switch(timetype) {
            case "Seconds":
                seconds = amount * 1;
                break;
            case "Minutes":
                seconds = amount * 60;
                break;
            default:
                seconds = amount;
                break;
        }

        this.gameData.ChangeSaveInterval(seconds);
    }
}
