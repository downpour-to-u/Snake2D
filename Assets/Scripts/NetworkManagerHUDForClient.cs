using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerHUDForClient : MonoBehaviour {

	public NetworkManager manager;
	public static string IpAddress;

	private long timeForNotConn;

	void Awake()
	{
		manager = GetComponent<NetworkManager>();
		manager.networkAddress = IpAddress;
		manager.StartClient();
		timeForNotConn = System.DateTime.Now.Ticks;
	}

	// Use this for initialization
	void Update () {
		if (manager.IsClientConnected ())
			return;
		else if (System.DateTime.Now.Ticks - timeForNotConn > 100000000)
			Application.LoadLevel ("lobby");
	}
}
