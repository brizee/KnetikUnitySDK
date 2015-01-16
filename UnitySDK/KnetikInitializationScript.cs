using UnityEngine;
using System.Collections;
using Knetik;

public class KnetikInitializationScript : MonoBehaviour {
	public static KnetikInitializationScript Singleton = null;

	public string BaseURL = "http://jsapi.dev:8080";
    public string ClientID = "fake_client";
	public string ClientSecret = "fake_secret";
    public string Authentication = "default";

	public Queue Requests = Queue.Synchronized( new Queue() );

	// Use this for initialization
	void Start () {
		if (Singleton) {
            return;
		}
		Singleton = this;
        KnetikClient.Instance.BaseURL = BaseURL;
        KnetikClient.Instance.ClientID = ClientID;
        KnetikClient.Instance.ClientSecret = ClientSecret;
        KnetikClient.Instance.Authentication = Authentication;

        DontDestroyOnLoad (gameObject);
	}

	public void Update()
	{
		while( Requests.Count > 0 )
		{
			KnetikRequest request = (KnetikRequest)Requests.Dequeue();
			request.completedCallback( request );
		}
	}
}
