using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerHUDForServer : MonoBehaviour {

	public NetworkManager manager;

	void Awake()
	{
		manager = GetComponent<NetworkManager>();
		manager.StartServer ();
		Debug.Log ("Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
	}
}
