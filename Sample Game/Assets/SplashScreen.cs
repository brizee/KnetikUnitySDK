using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {

	public Texture splashTexture;

	void OnGUI()
	{
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), splashTexture);
		}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			Application.LoadLevel(1);
				}
	}
}
