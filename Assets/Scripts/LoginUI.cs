using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUI : MonoBehaviour {

	public void loginOnClick () {
		
		Application.LoadLevel("lobby");
	}

	public void registerOnClick () {
		Application.LoadLevel("lobby");
	}
}
