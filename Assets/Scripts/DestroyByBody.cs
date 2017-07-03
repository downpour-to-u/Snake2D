using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class DestroyByBody : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		/*if (!isServer)
			return;*/
		if (other.tag == "Player") {
			// when the snakeHead(other) is not the body's head
			if (name.Split('(')[0] != other.name.Split('(')[0]) {
				// when the other is not defended;
				if (other.GetComponent<UserControl> ().isDefense () == false){
					GameObject[] listPlayer = GameObject.FindGameObjectsWithTag ("Player");
					for (int i = 0; i < listPlayer.Length; i++) {
						if (name.Split ('(') [0] == listPlayer [i].name) {
							// when the snake(the body belonged) is not denfended
							if (listPlayer [i].GetComponent<UserControl> ().isDefense () == false)
								GameObject.FindWithTag("GameController").GetComponent<GameController> ().Dead (other);
						}
					}
				}
			}
		}			
	}
}
