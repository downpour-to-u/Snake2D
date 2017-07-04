using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using CnControls;
public class UserControl : SnakeMove {
	public GameObject gDefense;	
	public GameObject gMagnetTrigger;
	public int initialBodyNum = 9;
	public int lenBody=1;
	public GameObject gRespawnUI;

	[SyncVar(hook = "OnChangeScore")]
	private int score = 0;

	[SyncVar]
	private int speedScale = 1;

	private bool defense = false;


	private long timerForDefense;

	private float dpf;
	private NetworkStartPosition[] spawnPoints;

	// Use this for initialization
	void Start () {
		//give current time to snake as name;
		name = System.DateTime.Now.Ticks + "";
		if (isLocalPlayer) {
			//for camera
			GameObject mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
			if (mainCamera)
				mainCamera.GetComponent<CameraControl> ().player = gameObject;
		}

		//for respawn
		spawnPoints = FindObjectsOfType<NetworkStartPosition>();			

		// *speed / FPS:distance the snake move in each fame;
		dpf = speed / FPS;
		// initializing the path; path[0] is the tail;
		for (float i =0; initialBodyNum - i*dpf > 0.0f; i++ ) {
			path.Insert(0,(Vector2)transform.position - new Vector2 (i *dpf, 0));
		}
		//creat body on the left of head;
		for (int i = 0; i < initialBodyNum; i++) {
			GameObject tmpBody = Instantiate (gBody, (Vector2)transform.position- new Vector2 (i*lenBody, 0) , new Quaternion());
			//tmpBody.transform.parent = transform;
			tmpBody.name = name;
			lstBody.Add (tmpBody);
		}
		StartDefense ();
	}
	
	void FixedUpdate()
	{
		if (transform.position.x >= 1000)
			return;
		//get the forward of head;
		Vector2 forward = new Vector2((float)Math.Cos(transform.eulerAngles.z*Math.PI/180),(float)Math.Sin(transform.eulerAngles.z*Math.PI/180));
		//speed up,move
		if (isLocalPlayer) {
			//change direction
			Vector2 dstMovement = new Vector2 (CnInputManager.GetAxis ("Horizontal"), CnInputManager.GetAxis ("Vertical"));
			Vector2 tmpMovement = Vector2.Lerp (forward, dstMovement, 0.125f);
			float arc = (float)Math.Atan2 (tmpMovement.y, tmpMovement.x);
			transform.rotation = Quaternion.Euler (0, 0, (float)(arc * 180.0f / Math.PI));

			if (CnInputManager.GetButton ("Accelerate")) {
				//server execute
				if(speedScale!=2)
					CmdSetSpeedScale(2);
			} else {
				if(speedScale!=1)
					CmdSetSpeedScale(1);
			}
		}
		if (speedScale == 2)
			Debug.Log ("speed");
		
		//add path
		for (int i = 1; i <= speedScale; i++) {
			path.Add ((Vector2)transform.position + forward * dpf * i);
		}
		//move head
		transform.position = new Vector2(path [path.Count - 1].x, path [path.Count - 1].y);
		//move bodies
		int lenBody_InPath = (int)Math.Round (lenBody*FPS / speed);//how many locs length of a body in path(list);
		for (int i = 0; i < lstBody.Count; i++) {
			Vector2 tmpPosition;
			/**********
			 path.Count-1: the last loc in the path;  
			 path.Count - 1 -lenBody_InPath: position of the second node should be (first is snake head);
			 Math.Min (path.Count -1 - lenBody_InPath, i * lenBody_InPath ): if path is not long enough(when the snake become long suddenly), choose the last node 
			 **********/
			tmpPosition = path [ path.Count - 1 -lenBody_InPath   - Math.Min (path.Count -1 - lenBody_InPath, i * lenBody_InPath )];
			lstBody[i].transform.position = new Vector2(tmpPosition.x,tmpPosition.y);
		}
		//remove unnecessary path node(save another two node for future "add body")
		if (path.Count > (lstBody.Count + 1) * lenBody_InPath + lenBody_InPath * 2) {
			path.RemoveAt (0);
		}

		if (isServer) {
			if (defense == true && System.DateTime.Now.Ticks - timerForDefense >= 30000000) {
				RpcCloseDefense ();
			}
		}
	}


	
	//Accelarate:server execute
	[Command]
	void CmdSetSpeedScale(int value){
		speedScale = value;
	}
		
	//add score
	override public void addScore(int _score){
		score=score+_score;
	}
	void OnChangeScore(int _socre){
		if(isLocalPlayer)
			GameObject.FindGameObjectWithTag ("TextLength").GetComponent<Text> ().text = "我的分数：" + _socre;
		//add body
		if(lstBody.Count - initialBodyNum>=0)
			while (_socre / 5 > lstBody.Count - initialBodyNum)
				addBody ();
	}
	public void addBody(){
		GameObject LastBody = lstBody [lstBody.Count - 1];
		Vector2 forward = new Vector2((float)Math.Cos(LastBody.transform.eulerAngles.z*Math.PI/180),
			(float)Math.Sin(LastBody.transform.eulerAngles.z*Math.PI/180));
		GameObject tmpBody = Instantiate (gBody, (Vector2)LastBody.transform.position - forward, 
			LastBody.transform.rotation);
		tmpBody.name = name;
		tmpBody.GetComponent<SpriteRenderer> ().sortingOrder = LastBody.GetComponent<SpriteRenderer> ().sortingOrder - 1;
		lstBody.Add (tmpBody);
		if (defense == true) {
			GameObject tmpGoDefense = Instantiate (gDefense, tmpBody.transform.position, new Quaternion ());
			tmpGoDefense.transform.parent = tmpBody.transform;
		}
		//increase the order
		//in case of the order become lower than the background
		transform.GetComponent<SpriteRenderer> ().sortingOrder++;
		for (int i = 0; i < lstBody.Count; i++) {
			lstBody [i].GetComponent<SpriteRenderer> ().sortingOrder++;
		}
	}


	//****************prop:1.megnet

	[ClientRpc]
	public void RpcStartMagnet(){
		GameObject tmpGMagnetTrigger = Instantiate (gMagnetTrigger, transform.position, new Quaternion ());
		tmpGMagnetTrigger.transform.parent = transform;
	}
	//2.defense
	public void StartDefense(){
		if(defense==true)
			timerForDefense = System.DateTime.Now.Ticks;
		else{
			defense = true;
			timerForDefense = System.DateTime.Now.Ticks;
			Instantiate (gDefense, transform.position, new Quaternion (),transform);
			for(int i =0;i<lstBody.Count;i++){
				Instantiate (gDefense, lstBody[i].transform.position, new Quaternion (),lstBody[i].transform);
			}
		}
	}
	public void CloseDefense(){
		defense = false;
		var de_head = transform.FindChild ("BodyDefense(Clone)");
		if(de_head)
			Destroy (de_head.gameObject);
		for (int i = 0; i < lstBody.Count; i++) {
			Destroy (lstBody[i].transform.FindChild("BodyDefense(Clone)").gameObject);
		}
	}
	public bool isDefense(){
		return defense;
	}
	[ClientRpc]
	public void RpcStartDefense(){
		StartDefense ();
	}
	[ClientRpc]
	void RpcCloseDefense(){
		CloseDefense ();
	}
	//*****************prop end


	//destroy
	public void DestroyBody(){
		if (!isServer)
			return;
		RpcDestroyBody ();
	}
	[ClientRpc]
	void RpcDestroyBody(){
		if (defense == true)
			CloseDefense ();
		for (int i = 0; i < lstBody.Count; i++)
			Destroy (lstBody [i]);
		lstBody.Clear ();
		score = 0;
		if (isLocalPlayer){
			MainUI.localplayer = gameObject;
			if (GameObject.FindGameObjectWithTag ("RespawnUI") == null)
				Instantiate (gRespawnUI, (Vector2)transform.position, new Quaternion ());
			CameraControl.isFollow = false;
		}
		//move head to (1000,1000) where player can not see, wait to respawn
		gameObject.transform.position = new Vector2 (1000, 1000);
	}	


	//Respawn
	[Command]
	public void CmdRespawn(){
		// Set the spawn point to origin as a default value
		Vector3 spawnPoint = Vector3.zero;

		// If there is a spawn point array and the array is not empty, pick one at random
		if (spawnPoints != null && spawnPoints.Length > 0) {
			spawnPoint = spawnPoints [UnityEngine.Random.Range (0, spawnPoints.Length)].transform.position;
		}
		RpcRespawn (spawnPoint);
	}

	[ClientRpc]
	public void RpcRespawn(Vector3 _spawnPoint){
		if (isLocalPlayer)
			CameraControl.isFollow = true;
		// Set the player’s position to the chosen spawn point
		transform.position = _spawnPoint;
		// initializing the path; path[0] is the tail;
		path.Clear();
		for (float i = 0; initialBodyNum - i * dpf > 0.0f; i++) {
			path.Insert (0, (Vector2)transform.position - new Vector2 (i * dpf, 0));
		}

		//creat body on the left of head;
		for (int i = 0; i < initialBodyNum; i++) {
			GameObject tmpBody = Instantiate (gBody, (Vector2)transform.position - new Vector2 (i * lenBody, 0), new Quaternion ());
			//tmpBody.transform.parent = transform;
			tmpBody.name = name;
			lstBody.Add (tmpBody);
		}
		StartDefense ();
	}

	//get
	override public List<GameObject> GetBody(){
		return lstBody;
	}
	public List<Vector2> GetPath(){
		return path;
	}
}
