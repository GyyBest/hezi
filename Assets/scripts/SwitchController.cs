using UnityEngine;
using System;
using System.Collections;

public class SwitchController : MonoBehaviour {
	[HideInInspector]
	public bool switchOn = false;

	private string tagCrate = "Crate";
	public Sprite[] switchStates;

	public AudioClip toggleSound;
    private PlayerController playerController;

	void Start () {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

	void OnTriggerEnter (Collider other) {
		if (other.tag == tagCrate) {
			switchOn = true;
            GameObject.Find("Level").GetComponent<GameManager>().CheckAllSwitchesState();
            GetComponent<AudioSource> ().PlayOneShot (toggleSound);
		}
        else if (other.tag == "Player") {
            playerController.onSwitch = true;
        }

		StartCoroutine (changeState ());
	}

	void OnTriggerExit (Collider other) {
		switchOn = false;
        if (other.tag == "Player") {
            playerController.onSwitch = false;
        }

        StartCoroutine (changeState ());
	}

	public IEnumerator changeState () {
		yield return new WaitForSeconds (0.20f);

		this.GetComponent<SpriteRenderer> ().sprite = switchStates [Convert.ToInt32 (switchOn)];
		yield return 0;
	}
}