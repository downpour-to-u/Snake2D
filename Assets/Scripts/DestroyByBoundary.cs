using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DestroyByBoundary : NetworkBehaviour {
	
	//[SyncVar(hook = "OnDead")]
	//NetworkInstanceId ID = NetworkInstanceId.Invalid;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!isServer)
			return;
		if (other.tag == "Player"){
			//NetworkInstanceId netWorkID = other.GetComponent<NetworkIdentity> ();
			GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ().Dead (other);
		}
	}

	/*
	void OnDead(string _head){
		GameObject[] listPlayer = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < listPlayer.Length; i++) {
			if (_head.Split('(')[0] == listPlayer [i].name)
				GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ().Dead (listPlayer [i].GetComponent<Collider2D>());
		}
	}*/
}
