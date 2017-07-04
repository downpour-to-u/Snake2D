using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : MonoBehaviour {
	public UILabel gName;
	public UIInput gIp;

	void Start(){
		gName.text = "昵称：" + SettingsUI.userName;
		gIp.isSelected = true;
		gIp.value = "localhost";
	}

	public void userOnClick () {
		//Application.LoadLevel("main");
	}

	public void storeOnClick () {
		Application.LoadLevel("store");
	}

	public void friendOnClick () {
		//Application.LoadLevel("main");
	}

	public void settingsOnClick () {
		Application.LoadLevel("settings");
	}

	public void singleOnClick () {
		Application.LoadLevel("main");
	}

	public void multiOnClick () {
		Application.LoadLevel("main");
		NetworkManagerHUDForClient.IpAddress = gIp.value;
	}
}
