using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByBody : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") {
			if (name.Split('(')[0] != other.name.Split('(')[0]) {
				if (other.name.Split('(')[0] != "Enemy" && other.GetComponent<UserControl> ().isDefense () == true)
					return;
				if (name.Split('(')[0] != "Enemy") {
					GameObject[] listPlayer = GameObject.FindGameObjectsWithTag ("Player");
					for (int i = 0; i < listPlayer.Length; i++) {
						if (name.Split ('(') [0] == listPlayer [i].name) {
							if (listPlayer [i].GetComponent<UserControl> ().isDefense () == true)
								return;
							break;
						}
					}
				}
				GameObject.FindWithTag("GameController").GetComponent<GameController> ().Dead (other);
			}
		}			
	}
}
