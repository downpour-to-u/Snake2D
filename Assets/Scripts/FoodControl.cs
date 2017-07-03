using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class FoodControl : NetworkBehaviour {
	public int score = 1;
	void OnTriggerEnter2D(Collider2D other) {
		if (!isServer)
			return;
		if (other.tag == "Player") {
			other.GetComponent<SnakeMove> ().addScore (score);
			GameObject.FindWithTag ("GameController").GetComponent<GameController> ().subFood ();
			Destroy (gameObject);
		}
	}
}
