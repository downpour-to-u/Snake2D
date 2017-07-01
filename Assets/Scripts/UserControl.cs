using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using CnControls;
public class UserControl : SnakeMove {
	public GameObject enemy;
	public GameObject gDefense;	
	public GameObject gMagnetTrigger;
	public int initialBodyNum = 9;

	[SyncVar(hook = "OnChangeLength")]
	private int score = 0;

	[SyncVar(hook = "OnChangeSpeedScale")]
	private int speedScale = 1;


	private bool defense = false;
	[SyncVar(hook = "OnChangeDefense")]
	private int record_defense=0;

	[SyncVar(hook = "OnChangeMagnet")]
	private int record_magnet=0;
	[SyncVar(hook = "OnDestroyBody")]
	private int record_destroy=0;

	private long timerForDefense;

	// Use this for initialization
	void Start () {
		//if(isServer)
		//	Instantiate (enemy, new Vector2 (5.4f, 3.9f), new Quaternion ());

		name = System.DateTime.Now.Ticks + "";
		if (isLocalPlayer) {
			GameObject mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
			if (mainCamera)
				mainCamera.GetComponent<CameraControl> ().player = gameObject;
		}

		for (float i = initialBodyNum + 1.0f; i > 0.0f; i -= 2.0f / FPS) {
			path.Add ((Vector2)transform.position - new Vector2 (i + 1, 0));
		}
		for (int i = 0; i < initialBodyNum; i++) {
			GameObject tmpBody = Instantiate (gBody, (Vector2)transform.position - new Vector2 (i + 1, 0), new Quaternion());
			//tmpBody.transform.parent = transform;
			tmpBody.name = name;
			lstBody.Add (tmpBody);
		}
		OnChangeDefense (0);
	}
	
	void FixedUpdate()
	{
		Vector2 forward = new Vector2((float)Math.Cos(transform.eulerAngles.z*Math.PI/180),(float)Math.Sin(transform.eulerAngles.z*Math.PI/180));
		if (isLocalPlayer) {
			//speed up
			if (CnInputManager.GetButton ("Accelerate")) {
				speedScale = 2;
			} else {
				speedScale = 1;
			}
		}
		//add path
		for (int i = 1; i <= speedScale; i++) {
			Vector2 tmpPosition = (Vector2)transform.position + forward * (speed / FPS) * i;
			path.Add (new Vector2 (tmpPosition.x, tmpPosition.y));
		}
		//move head
		transform.position = new Vector2(path [path.Count - 1].x, path [path.Count - 1].y);
		//move bodies
		int lag = (int)Math.Round ((FPS/speedScale) / speed);
		for (int i = 0; i < lstBody.Count; i++) {
			Vector2 tmpPosition;
			if (speedScale == 1)
				tmpPosition = path [path.Count - lag - 1 - Math.Min (path.Count - lag - 1, i * lag)];
			else
				tmpPosition = path [path.Count - (int)(lag * speedScale) - 1 - Math.Min (path.Count - (int)(lag * speedScale) - 1, i * (int)(lag * speedScale))];
			lstBody[i].transform.position = new Vector2(tmpPosition.x,tmpPosition.y);
		}
		//remove unnecessary path node
		if (path.Count > (lstBody.Count + 1) * lag + lag * 2) {
			path.RemoveAt (0);
		}

		if (isLocalPlayer) {
			//change direction
			Vector2 dstMovement = new Vector2 (CnInputManager.GetAxis ("Horizontal"), CnInputManager.GetAxis ("Vertical"));
			Vector2 tmpMovement = Vector2.Lerp (forward, dstMovement, 0.125f);
			float arc = (float)Math.Atan2 (tmpMovement.y, tmpMovement.x);
			transform.rotation = Quaternion.Euler (0, 0, (float)(arc * 180.0f / Math.PI));
		}


		//close defense if time is out8
		if (defense == true && System.DateTime.Now.Ticks - timerForDefense >= 30000000) {
			defense = false;
			var de = transform.FindChild ("BodyDefense(Clone)");
			if(de)
				Destroy (de.gameObject);
			for (int i = 0; i < lstBody.Count; i++) {
				Destroy (lstBody[i].transform.FindChild("BodyDefense(Clone)").gameObject);
			}
		}
	}

	/*void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "body") {
			for (int i = 0; i < lstBody.Count; i++) {
				Destroy (lstBody [i]);
			}
			lstBody.Clear ();
			path.Clear ();
			Destroy (gameObject);
			//Application.LoadLevel(Application.loadedLevel);
		}
	}*/


	override public void addScore(int _score){
		score += _score;
		if (isLocalPlayer)
			GameObject.FindGameObjectWithTag ("TextLength").GetComponent<Text> ().text = "我的分数：" + score;
		//add body
		while (score / 5 > lstBody.Count - initialBodyNum)
			addBody ();
	}

	public void addBody(){
		Vector2 forward = new Vector2((float)Math.Cos(transform.eulerAngles.z*Math.PI/180),(float)Math.Sin(transform.eulerAngles.z*Math.PI/180));
		GameObject tmpBody = Instantiate (gBody, (Vector2)transform.position - forward, transform.rotation);
		//tmpBody.transform.parent = transform;
		tmpBody.name = name;
		lstBody.Add (tmpBody);
		if (defense == true) {
			GameObject tmpGoDefense = Instantiate (gDefense, tmpBody.transform.position, new Quaternion ());
			tmpGoDefense.transform.parent = tmpBody.transform;
		}
	}

	override public void Destroy(){
		Destroy (gameObject);
	}

	override public List<GameObject> GetBody(){
		return lstBody;
	}

	public bool isDefense(){
		return defense;
	}
		
	public void startDefense(){
		if (!isServer)
			return;
		record_defense++;

	}
	public void startMagnet(){
		if (!isServer)
			return;
		record_magnet++;

	}
	public void DestroyBody(){
		if (!isServer)
			return;
		record_destroy++;

	}


	void OnChangeLength(int _score){
		score = _score;
	}

	void OnChangeSpeedScale(int _speedScale){
		speedScale = _speedScale;
	}

	void OnChangeDefense(int r_d){
		if(defense==true)
			timerForDefense = System.DateTime.Now.Ticks;
		else{
			defense = true;
			/*var de = transform.FindChild ("BodyDefense(Clone)");
		if (de)
			return;*/
			timerForDefense = System.DateTime.Now.Ticks;
			GameObject goDefense = Instantiate (gDefense, transform.position, new Quaternion ());
			goDefense.transform.parent = transform;
			for(int i =0;i<lstBody.Count;i++){
				GameObject tmpGDefenseLb = Instantiate (gDefense, lstBody[i].transform.position, new Quaternion ());
				tmpGDefenseLb.transform.parent = lstBody[i].transform;
			}
		}

	}

	void OnChangeMagnet(int r_m){
		GameObject tmpGMagnetTrigger = Instantiate (gMagnetTrigger, transform.position, new Quaternion ());
		tmpGMagnetTrigger.transform.parent = transform;
	}
	void OnDestroyBody(int r_d){
		Debug.Log (r_d);
		for (int i = 0; i < lstBody.Count; i++)
			Destroy (lstBody [i]);
		gameObject.SetActive (false);
	}
}
