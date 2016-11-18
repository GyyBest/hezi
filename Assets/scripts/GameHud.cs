using UnityEngine;
using System.Collections;

public class GameHud : MonoBehaviour {
	int moves = 0;
	int pushes = 0;
    [HideInInspector]
    public bool finish = false;

	void Start () {
		Reset ();
	}

	public void Reset () {
		moves = 0;
		pushes = 0;
	}

	public int GetCountMoves () {
		return moves;
	}

	public int GetCountPushes () {
		return pushes;
	}

	public void IncreaseMoves () {
		moves++;
	}

	public void IncreasePushes () {
		pushes++;
	}

	void Update () {
        string txt = "Moves: " + moves + "\n" + "Pushes: " + pushes;
        if (finish)
            txt += "\nYou won !";
        GetComponent<GUIText> ().text = txt;
	}
}