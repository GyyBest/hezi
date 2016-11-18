using UnityEngine;
using System.Collections;
using UnityEngine.UI; //Need this for calling UI scripts
using UnityEngine.SceneManagement;//use the method SceneManager.LoadScene

[RequireComponent(typeof(AudioSource))]

public class ManagerButton : MonoBehaviour {

	[SerializeField]
	Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it
	//public GameObject[] UIPanel;
	[SerializeField]
	Text timeText; //Will assign our Time Text to this variable so we can modify the text it displays.


	public bool isPaused = true; //Used to determine paused state
	public AudioClip click;


	void Start ()
	{
		
		//timeText.text = "Time Since Startup: " + Time.timeSinceLevelLoad; //Tells us the time since the scene loaded
		
		UIPanel.gameObject.SetActive(true);//make sure our pause menu is disabled when scene starts
		isPaused = true; //make sure isPaused is always false when our scene opens

	}

	void Update ()
	{
		//If player presses escape and game is not paused. Pause game. If game is paused and player presses escape, unpause.
		if(Input.GetKeyDown(KeyCode.Escape) && !isPaused)
			Pause();
		else if(Input.GetKeyDown(KeyCode.Escape) && isPaused)
			Resume();
	}

	public void Pause()
	{
		isPaused = true;
		//GameObject.Find ("PausedPanel").SetActive (true);
		UIPanel.gameObject.SetActive(true); //turn on the pause menu
		Time.timeScale = 0f; //pause the game
	}

	public void Resume()
	{
		isPaused = false;
		//GameObject.Find ("PausedPanel").SetActive (true);
		UIPanel.gameObject.SetActive(false); //turn off pause menu
		SceneManager.LoadSceneAsync(1);
		Time.timeScale = 1f; //resume game
	}

	public void QuitGame()
	{
		Application.Quit();
	}
		
	public void restart()
	{
		SceneManager.LoadScene (1);
	}

	public void ClickSound()
	{
		GetComponent<AudioSource> ().PlayOneShot(click);
	}
}