using UnityEngine;
using System.Collections;
using Knetik;
using System;

public class UserDisplayClass : MonoBehaviour {
	public GUIText BombCounter;
	public static int sharksDestroyedCount;
	public static int bombsLeft;
	public static string bombed;
	
	// Use this for initialization
	void Start () {
		/****************************************
		// KNETIK-API
		****************************************/

		// PlayVS SAPI
		ApiUtil.setApiHost("dev.sapi.playvs.net");
		ApiUtil.setClientKey("test_key");
		ApiUtil.setClientSecret("test_secret");

		/****************************************
		// KNETIK-API
		****************************************/

		sharksDestroyedCount = 0;
		bombsLeft = 30;
		bombed = null;
		BombCounter = GameObject.Find("BombCounter").guiText;
	}
	
	// Update is called once per frame
	void Update () {
		if(bombed == null)
		{
			BombCounter.guiText.text = "Sharks Bombed: " + sharksDestroyedCount  + "\n" + "Bombs Left:" + bombsLeft;
		}
		else
		{
			BombCounter.guiText.text = "Sharks Bombed: " + bombed  + "\n" + "Bomb\ts Left:" + bombsLeft;
		}
	}
	
	
}
