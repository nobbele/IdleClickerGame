using UnityEngine;
using UnityEngine.UI;

public class OpenNewMenuBtn : MonoBehaviour
{
    [SerializeField]
    string CanvasToOpen;
    // Use this for initialization
    void Start() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick() {
        GameData.ChangeCanvas(this.CanvasToOpen);
    }
}
