using UnityEngine;
using System.Collections;
using Knetik;
using System;

public class BombClass : MonoBehaviour {
	private float ySpeed = -4f;
	public GameObject explosionPrefab;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate( new Vector3(0f, ySpeed*Time.deltaTime, 0f) );
		if (transform.position.y < -11) {
			Destroy(this.gameObject);
		}
	}
	
	void OnTriggerEnter(Collider obj) {
		if (obj.gameObject.name == "Shark") {
			//reset shark
			obj.gameObject.transform.rotation = Quaternion.identity;
			obj.gameObject.transform.position = new Vector3(20f, -3f, 8f);
			Destroy(this.gameObject);
			Instantiate (explosionPrefab, transform.position, Quaternion.identity);
			UserDisplayClass.sharksDestroyedCount++;

			/****************************************
			// KNETIK-API
			****************************************/

			// Post a Game Event, using the Hash ID from iOS and a desired score
			// 		Game Events can be called any number of times on the same Hash ID

			string sessionKey = "7193d91121e9a58f3577ccd86292cf0c";

			string eventHashId = "7cfe2104935351c8943bd95ce88f3aacee57bc97";
			string eventScore = "400";
			GameEventRequest ge = new GameEventRequest(sessionKey, eventHashId, eventScore);
			// Use a delegate call to run GameEventRequest and postGameEvent async
			new Action<GameEventRequest>(PostGameEvent).BeginInvoke(ge, null, null);

			// Post a Game Result, using the Hash ID from iOS and a desired score
			// 		The Result will be the final score call and the score for this Hash ID
			// 		cannot be updated after this call
//			string resultHashId = "bf60dd80c920f4a7cbae9bf123e95d46dcba85d8";
//			string resultScore = "1000";
//			GameEventRequest gr = new GameEventRequest(sessionKey, resultHashId, resultScore);
//			// Use a delegate call to run GameEventRequest and postGameEvent async
//			new Action<GameEventRequest>(PostGameResult).BeginInvoke(gr, null, null);

			/****************************************
			// KNETIK-API
			****************************************/

		}
	}

	/****************************************
	// KNETIK-API
	****************************************/
	void PostGameEvent(GameEventRequest ge)
	{
		string mode = "event";
		if(ge.postGameEvent(mode)) {
			Debug.Log("Game Event Post Successful");
		} else {
			Debug.Log("Game Event Post FAILED!!!");
		}		
	}

	void PostGameResult(GameEventRequest gr)
	{
		string mode = "result";
		if(gr.postGameEvent(mode)) {
			Debug.Log("Game Result Post Successful");
		} else {
			Debug.Log("Game Result Post FAILED!!!");
		}		
	}
	/****************************************
	// KNETIK-API
	****************************************/
}