using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
	public GameObject player;

	public static bool isFollow = true;

	// Use this for initialization
	void Start () {
		this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null && isFollow)
			this.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, -10.0f);
	}
}
