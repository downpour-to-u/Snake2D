using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetTrigger : MonoBehaviour {
	private long timer;
	void Start(){
		//timer = System.DateTime.Now.Ticks;
		Destroy(gameObject,10.0f);
	}

	/*void Update(){
		if (System.DateTime.Now.Ticks - timer >= 50000000) {
			Destroy (gameObject);
		}
	}*/

	void OnTriggerStay2D  (Collider2D other) {
		if (other.name.Contains("food") || other.name.Contains("prop") || other.name.Contains("DeadBody")) {
			other.GetComponent<Rigidbody2D> ().velocity = (gameObject.transform.position - other.transform.position)*10.0f;
		}
	}
	void OnTriggerExit2D  (Collider2D other) {
		if (other.name.Contains("food") || other.name.Contains("prop") || other.name.Contains("DeadBody")) {
			other.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		}
	}
}
