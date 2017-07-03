using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUI : MonoBehaviour {
	public static string userName = "snake007";
	public static bool isBgm = true;
	public static bool isMusic = true;
	public UIInput gName;

	void Start(){
		gName.value = userName;
	}

	public void returnToLobby(){
		Application.LoadLevel("lobby");
	}

	public void changeName(){
		userName = gName.value;
	}

	public void changeBgm(UIToggle gBgm){
		isBgm = gBgm.value;
	}

	public void changeMusic(UIToggle gMusic){
		isMusic = gMusic.value;
	}
}
