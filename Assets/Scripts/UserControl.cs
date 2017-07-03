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

	[SyncVar(hook = "OnChangeScore")]
	private int score = 0;

	[SyncVar]
	private int speedScale = 1;

	private bool defense = false;
	[SyncVar(hook = "OnChangeDefense")]
	private int record_defense=0;

	[SyncVar(hook = "OnChangeMagnet")]
	private int record_magnet=0;
	[SyncVar(hook = "OnDestroyBody")]
	private int record_destroy=0;

	private long timerForDefense;

	private string id;
	// distance the snake move in each fame;
	private float dpf;
	GameObject test;
	// Use this for initialization
	void Start () {
		//give current time to snake as name;
		Debug.Break ();
		if (isLocalPlayer) {
			test = gDefense;
			GameObject mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
			if (mainCamera)
				mainCamera.GetComponent<CameraControl> ().player = gameObject;
			

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
			OnChangeDefense (0);
			id = System.DateTime.Now.Ticks + "";
		}
		Debug.Log (id);

	}
	
	void FixedUpdate()
	{
		//get the forward of head;
		Vector2 forward = new Vector2((float)Math.Cos(transform.eulerAngles.z*Math.PI/180),(float)Math.Sin(transform.eulerAngles.z*Math.PI/180));
		//speed up
		if (isLocalPlayer) {
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
			Vector2 tmpPosition = (Vector2)transform.position + forward * dpf * i;
			Vector2 newp = new Vector2 (tmpPosition.x, tmpPosition.y);
			path.Add (newp);
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

		if (isLocalPlayer) {
			//change direction
			Vector2 dstMovement = new Vector2 (CnInputManager.GetAxis ("Horizontal"), CnInputManager.GetAxis ("Vertical"));
			Vector2 tmpMovement = Vector2.Lerp (forward, dstMovement, 0.125f);
			float arc = (float)Math.Atan2 (tmpMovement.y, tmpMovement.x);
			transform.rotation = Quaternion.Euler (0, 0, (float)(arc * 180.0f / Math.PI));
		}


		//close defense if time is out8
		if (defense == true && System.DateTime.Now.Ticks - timerForDefense >= 50000000) {
			defense = false;
			var de = transform.FindChild ("BodyDefense(Clone)");
			if(de)
				Destroy (de.gameObject);
			for (int i = 0; i < lstBody.Count; i++) {
				Destroy (lstBody[i].transform.FindChild("BodyDefense(Clone)").gameObject);
			}
		}

	}

	//server execute;
	[Command]
	void CmdSetSpeedScale(int value){
		speedScale = value;
	}
		

	override public void addScore(int _score){
		score=score+_score;
		if (isLocalPlayer)
			return;
		OnChangeScore (score);
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
		if (isLocalPlayer)
			return;
		OnChangeDefense (0);

	}
	public void startMagnet(){
		if (!isServer)
			return;
		record_magnet++;
		if (isLocalPlayer)
			return;
		OnChangeMagnet (0);
	}
	public void DestroyBody(){
		if (!isServer)
			return;
		record_destroy++;
		if (isLocalPlayer)
			return;
		OnDestroyBody (0);

	}
		
	void OnChangeScore(int _socre){
		GameObject.FindGameObjectWithTag ("TextLength").GetComponent<Text> ().text = "我的分数：" + _socre;
		//add body
		while (_socre / 5 > lstBody.Count - initialBodyNum)
			addBody ();
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
		for (int i = 0; i < lstBody.Count; i++)
			Destroy (lstBody [i]);
		gameObject.SetActive (false);
	}
}
