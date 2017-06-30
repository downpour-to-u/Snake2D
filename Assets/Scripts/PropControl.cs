using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PropControl : MonoBehaviour {
	public GameObject gMagnetTrigger;
	public float speed = 1.5f;
	public float flexible = 0.5f;
	public int type = 1;//1 means star, 2 means ...

	private int FPS = 40;
	private Vector2 forward;
	// Use this for initialization
	void Start () {
		forward = new Vector2 (1, 0);		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//move
		transform.position = (Vector2)transform.position + forward * (speed / FPS);
	}

	void OnTriggerEnter2D(Collider2D co){
		if (co.tag == "Player") {
			if (type == 1) {
				co.GetComponent<UserControl> ().addScore (15);
			} else if (type == 2) {
				GameObject tmpGMagnetTrigger = Instantiate (gMagnetTrigger, co.transform.position, new Quaternion ());
				tmpGMagnetTrigger.transform.parent = co.transform;
			} else if (type == 3) {
				if (co.name.Split('(')[0] != "Enemy")
					co.GetComponent<UserControl> ().startDefense ();					
			}
			Destroy (gameObject);
		} else if (co.tag == "BoundaryX") {
			forward.x = -forward.x;
		} else if (co.tag == "BoundaryY") {
			forward.y = -forward.y;
		} else {
			if (UnityEngine.Random.Range (0.0f, 1.0f) < flexible) {
				forward.x = UnityEngine.Random.Range (-1.0f, 1.0f);
				forward.y = UnityEngine.Random.Range (-1.0f, 1.0f);
				forward.Normalize ();
			}
		}
	}
}
