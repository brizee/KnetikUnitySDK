using UnityEngine;
using System.Collections;

public class UserSession : MonoBehaviour {

	public string username;
	public int user_id;
	public string api_key;
	
	void Awake() {
	    // Do not destroy this game object:
	    DontDestroyOnLoad(this);
	}	
}
