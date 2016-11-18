using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    private GameObject[] switches;
    private GameHud gameHud;

	// Use this for initialization
	void Start () {
        switches = GameObject.FindGameObjectsWithTag("Switch");
        gameHud = GameObject.Find("GameHud").GetComponent<GameHud>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CheckAllSwitchesState() {
        bool oneSwitchOff = false;
        foreach (GameObject switchObj in switches) {
            SwitchController switchController = switchObj.GetComponent<SwitchController>();
            if (!switchController.switchOn) {
                oneSwitchOff = true;
                break;
            }
        }
        if (!oneSwitchOff)
            gameHud.finish = true;
    }
}