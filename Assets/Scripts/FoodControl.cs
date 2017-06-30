using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodControl : MonoBehaviour {
	public int score = 1;
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			other.GetComponent<SnakeMove> ().addScore (score);
			GameObject.FindWithTag ("GameController").GetComponent<GameController> ().subFood ();
			Destroy (gameObject);
		}
	}
}
