using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour {
	public static GameObject localplayer;
	public void respawnOnClick(){
		localplayer.SetActive (true);
		localplayer.GetComponent<UserControl> ().RpcRespawn ();
		Destroy (gameObject);
	}
}
