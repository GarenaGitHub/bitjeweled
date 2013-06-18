using UnityEngine;
using System.Collections;

public class LevelSetter : MonoBehaviour {

	void LevelSelected () {
		// WARNING:
		// This is just an example, in real games you don't want to write
		// code like this... without any error checking ;)
		UILabel lbl = gameObject.GetComponentInChildren<UILabel>();
		int selection = int.Parse(lbl.text);
		Board.BoardNumber = selection;
		// This is a way good like any other to reduce the available time
		// in minutes depending on the level number. 
		// To be checked against the actual # of levels you have. Here we have 28, so
		// we go from 10m (level 1) down to 2m53s (level 28).
		Board.LevelTime = 11f / (1f+((float)(selection*10f)/100f));
		Application.LoadLevel("game");
	}
}
