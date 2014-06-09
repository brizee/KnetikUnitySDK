using UnityEngine;
using System.Collections;

public class FishClass : MonoBehaviour {
	public float speed = 6f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x < -19 || transform.position.x > 20) {
			//turn around
			transform.Rotate(new Vector3(0,180,0));
			if(gameObject.name == "Shark") {
				transform.Translate( new Vector3(-5, -transform.position.y + Random.Range(-4,-1),0) );
			} else if (gameObject.name == "ClownFish") {
				transform.Translate( new Vector3(-5, -transform.position.y + Random.Range(-1,1),0) );
			}
			//get new speed
			speed = Random.Range(6f,10f);
		}
		transform.Translate( new Vector3(-speed*Time.deltaTime,0,0) );	
	}
}
