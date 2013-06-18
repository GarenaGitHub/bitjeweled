using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	public UITable selectionTable;
	public GameObject levelSelectorPrefab;
	public int maxBoards = 10;
	public static int MaxBoards = 10;
	
	public TweenPosition[] mainMenu;
	public TweenPosition[] levelSelection;
	
	private int currentLevel = 1;
	
	void Start()
	{
		// We set this just out of habit because sometimes in between scenes 
		// the timescale driven by the the monobehaviour event isn't reliable.
		Time.timeScale = 1f;
		
		MaxBoards = maxBoards;
		Board.BoardNumber = 1; // just to be sure that if the user hits Play not selecting anything the #1 is there
		currentLevel = PlayerPrefs.GetInt ("currentLevel", 1);
		for (int i = 1;	i <= maxBoards; i++)
		{
			GameObject tmp = Instantiate(levelSelectorPrefab) as GameObject;
			tmp.transform.parent = selectionTable.transform;
			tmp.transform.localScale = Vector3.one;
			if (i>=10)
				tmp.name = i.ToString();
			else
				tmp.name = "0" + i.ToString();
			tmp.GetComponentInChildren<UILabel>().text = i.ToString();
			if (currentLevel < i)
			{
				tmp.GetComponentInChildren<UISprite>().color = Color.red;
				tmp.GetComponent<Collider>().enabled = false;
			}
		}
		selectionTable.Reposition();
	}
	
	/// <summary>
	/// Shows the Level Selection UI.
	/// </summary>
	void SelectLevel()
	{
		TweenUIElements(ref mainMenu, false);
		TweenUIElements(ref levelSelection, true);
	}
	
	/// <summary>
	/// Tweens the user interface elements. This is an utility method to allow
	/// a simpler approach to UI tweening keeping all the elements in just one panel
	/// so to have just 1 draw call
	/// </summary>
	/// <param name='items'>
	/// The UI Items.
	/// </param>
	/// <param name='forward'>
	/// True = Forward, False = Backward
	/// </param>
	void TweenUIElements(ref TweenPosition[] items, bool forward)
	{
		for (int i = 0; i < items.Length; i++)
			items[i].Play(forward);
	}
}
