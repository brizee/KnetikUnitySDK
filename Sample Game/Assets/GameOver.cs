using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	public Texture gameOverTexture;
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),gameOverTexture);
		GUI.Label(new Rect(Screen.width /2, Screen.height /2 - 50, 150, 25), "Game Over!");
		GUI.Label(new Rect(Screen.width /2, Screen.height /2 - 25, 200, 25), "You ran out of barrels! Score: " + UserDisplayClass.sharksDestroyedCount );
		if (GUI.Button(new Rect(Screen.width / 2, Screen.height /2, 150, 25),"Try Again")) 
		{
			Application.LoadLevel(2);
		}
		if (GUI.Button(new Rect(Screen.width / 2, Screen.height /2 + 25, 150, 25),"Quit")) 
		{
			Application.Quit();
		}
	}
}
