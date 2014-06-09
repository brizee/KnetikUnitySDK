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
						obj.gameObject.transform.position = new Vector3 (18f, -3f, 8f);
						Destroy (this.gameObject);
						Instantiate (explosionPrefab, transform.position, Quaternion.identity);
						UserDisplayClass.sharksDestroyedCount++;
						PostMetricRequest ur = new PostMetricRequest (UserSessionUtils.getApiKey (), 24, 43);
						PostMetricRequest ul = new PostMetricRequest (UserSessionUtils.getApiKey (), 24, 43, 1);
						// Use a delegate call to run PostMetricRequest and doPostMetric async
						new Action<PostMetricRequest> (PostMetric).BeginInvoke (ur, null, null);
						new Action<PostMetricRequest> (PostMetric).BeginInvoke (ul, null, null);
						DateTime time = DateTime.Now;              // Use current time
						string format = "MMM ddd d HH:mm:ss.fff yyyy";    // Use this format
						PostUserOptionsRequest userInsert = new PostUserOptionsRequest (UserSessionUtils.getApiKey (), "116", 43, "BOMBED!", time.ToString (format));
						userInsert.postUserInfo ("insert");
		} else if (obj.gameObject.name == "ClownFish") {
			//reset fish
			obj.gameObject.transform.rotation = Quaternion.identity;
			obj.gameObject.transform.position = new Vector3 (18f, 0.5f, 7f);
			Destroy (this.gameObject);
			Instantiate (explosionPrefab, transform.position, Quaternion.identity);	
		}

	}
	
	void PostMetric(PostMetricRequest ur)
	{
		if(ur.doPostMetric()) {
			Debug.Log("Metric post successful");
		} else {
			Debug.Log("METRIC POST FAILED!!!");
		}		
	}
}