using UnityEngine;
using System.Collections;

public class UserSessionUtils : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static string getApiKey() {
        return findUserSession().api_key;
    }
	
	public static string getUsername() {
        return findUserSession().username;
    }

    public static void setUserSession(int id, string name, string key) {
		findUserSession().user_id = id;
		findUserSession().username = name;
		findUserSession().api_key = key;
    }
    
    private static UserSession findUserSession() {
        GameObject userSession = GameObject.Find("UserSession");
        return (UserSession) userSession.GetComponent("UserSession");
    }	
}
