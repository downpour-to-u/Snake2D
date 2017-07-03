using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreUI : MonoBehaviour {
	public static int iSkin = 1;

	public void returnToLobby(){
		Application.LoadLevel("lobby");
	}

	public void changeSkin(GameObject gSkin, GameObject gScrollView){
		for (int i = 0; i < gScrollView.transform.childCount; i++) {
			gScrollView.transform.GetChild (i).FindChild ("Sprite").gameObject.SetActive (false);
		}
		gSkin.transform.Find("Sprite").gameObject.SetActive (true);
		iSkin = int.Parse (gSkin.name.Substring (7, gSkin.name.Length - 7));
		Debug.Log (iSkin);
	}
}
