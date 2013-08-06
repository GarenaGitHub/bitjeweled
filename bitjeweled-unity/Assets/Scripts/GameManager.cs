using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	private UILabel messageLabel;
	private TweenPosition gameOverPanel;
	private TweenPosition pausePanel;

	// Use this for initialization
	void Start () {
		// Here we can add code to change the presentation depending on the board number.
		// We can change the board texture, change the mesh, the background and so on.
		
		// Now for demonstration we just set a random back colour
		Vector3 tmp = Random.insideUnitSphere;
		Camera.mainCamera.backgroundColor = new Color(tmp.x, tmp.y,tmp.x);
		
		// This is just a demo, so I retrieve this straight away without any double check
		// as I know that the object is there and has that name: it can't be otherwise here :)
		messageLabel = GameObject.Find("MessageLabel").GetComponent<UILabel>();
		// Let's clear the label
		messageLabel.text = string.Empty;
		
		// This is just a demo, so I retrieve this straight away without any double check
		// as I know that the object is there and has that name: it can't be otherwise here :)
		gameOverPanel = GameObject.Find("GameOverPanel").GetComponent<TweenPosition>();
		// This is just a demo, so I retrieve this straight away without any double check
		// as I know that the object is there and has that name: it can't be otherwise here :)
		pausePanel = GameObject.Find("PausePanel").GetComponent<TweenPosition>();

		// Let's set the delegates
		Board.Instance.CheckBonusMethod = CheckBonus;
		Board.Instance.TimedOutMethod = TimeOut;
		
		// Now we start the board itself
		Board.Instance.StartBoard();
	}
	
	void ToMenu()
	{
		Time.timeScale = 1f;
		Application.LoadLevel("menu");
	}
	
	void UnPause()
	{
		Time.timeScale = 1f;
		Board.GamePaused = false;
	}
	
    void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus) {
			pausePanel.Play(true);
			Pause ();
		}
    }

	void Pause()
	{
		Time.timeScale = 0f;
		Board.GamePaused = true;
	}
	
	/// <summary>
	/// Time-out delegate.
	/// </summary>
	void TimeOut()
	{
		Time.timeScale = 0f;
        ApiManager api = FindObjectOfType(typeof(ApiManager)) as ApiManager;
        api.OnFinishedGame(ScoresManager.CurrentPoints);
        Debug.Log("calling finished game");
		gameOverPanel.Play(true);
	}
	
	/// <summary>
	/// Level cleared delegate.
	/// </summary>
	void LevelCleared()
	{
		Board.GamePaused = true;
		Board.BoardNumber++;
		if (Board.BoardNumber <= MenuManager.MaxBoards) {
			if (PlayerPrefs.GetInt ("currentLevel", 1) < Board.BoardNumber)
				PlayerPrefs.SetInt ("currentLevel", Board.BoardNumber);
			Board.Instance.StartBoard();
		} else {
			// well... getting here means that the player has beaten the game
			Application.LoadLevel("menu");
		}
	}

	/// <summary>
	/// Checks the bonus. This is our delegate to act on the bonus check.
	/// </summary>
	void CheckBonus()
	{
		if (Board.totalDestroyedPieces > 5)
		{
			ShowMessage("WOW! you cleared " + Board.totalDestroyedPieces.ToString() + " pieces in one shot!!!");
		}
	}
	
#region Utilities
	/// <summary>
	/// Clears the message label after 3 seconds.
	/// </summary>
	/// <returns>
	/// The message.
	/// </returns>
	IEnumerator ClearMessage()
	{
		yield return new WaitForSeconds(3f);
		messageLabel.text = string.Empty;
	}
	
	/// <summary>
	/// Shows the message.
	/// </summary>
	/// <param name='message'>
	/// Message.
	/// </param>
	void ShowMessage(string message)
	{
			messageLabel.text = message;
			StartCoroutine(ClearMessage());
	}
#endregion
	
}
