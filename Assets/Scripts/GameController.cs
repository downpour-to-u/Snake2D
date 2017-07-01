using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {
	public GameObject deadSnake;
	public GameObject food;
	public GameObject gStar;
	public GameObject gDefense;
	public GameObject gMagnet;
	public int maxFoodCount = 200;
	public float backgroundX;
	public float backgroundY;

	private int foodCount = 0;
	// Use this for initialization
	void Start () {
		if (!isServer)
			return;
		for (int i = 1; i <= maxFoodCount; i++) {
			GameObject tmpFood = Instantiate (food, new Vector2 (Random.Range (-backgroundX, backgroundX), Random.Range (-backgroundY, backgroundY)), new Quaternion ());
			NetworkServer.Spawn (tmpFood);
		}
		foodCount = maxFoodCount;
		GameObject tmpProp= Instantiate (gStar, new Vector2 (Random.Range (-backgroundX, backgroundX), Random.Range (-backgroundY, backgroundY)), new Quaternion ());
		NetworkServer.Spawn (tmpProp);
		GameObject tmpProp1 = Instantiate (gDefense, new Vector2 (Random.Range (-backgroundX, backgroundX), Random.Range (-backgroundY, backgroundY)), new Quaternion ());
		NetworkServer.Spawn (tmpProp1);
		GameObject tmpProp2 = Instantiate (gMagnet, new Vector2 (Random.Range (-backgroundX, backgroundX), Random.Range (-backgroundY, backgroundY)), new Quaternion ());
		NetworkServer.Spawn (tmpProp2);	


	}

	// Update is called once per frame
	void Update () {
		/*frame++;
		frame = frame % 600;
		if (frame == 1) {
			GameObject tmpProp;
			tmpProp= Instantiate (gStar, new Vector2 (Random.Range (-backgroundX, backgroundX), Random.Range (-backgroundY, backgroundY)), new Quaternion ());
			NetworkServer.Spawn (tmpProp);
			tmpProp = Instantiate (gDefense, new Vector2 (Random.Range (-backgroundX, backgroundX), Random.Range (-backgroundY, backgroundY)), new Quaternion ());
			NetworkServer.Spawn (tmpProp);
			tmpProp = Instantiate (gMagnet, new Vector2 (Random.Range (-backgroundX, backgroundX), Random.Range (-backgroundY, backgroundY)), new Quaternion ());
			NetworkServer.Spawn (tmpProp);	
		}*/
			
		if (!isServer)
			return;
		if (foodCount < maxFoodCount) {
			GameObject tmpFood = Instantiate (food, new Vector2 (Random.Range (-backgroundX, backgroundX), Random.Range (-backgroundY, backgroundY)), new Quaternion ());
			NetworkServer.Spawn (tmpFood);
			foodCount++;
		}
	}

	public void subFood(){
		foodCount--;
	}

	public void Dead(Collider2D player){
		SnakeMove userControl= player.GetComponent<SnakeMove> ();
		//UserControl userControl= new GhostMove ();
		Instantiate (deadSnake, (Vector2)player.transform.position+new Vector2(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f)), player.transform.rotation);
		List<GameObject> listBody = userControl.GetBody();
		for (int i = 0; i < listBody.Count; i++) {
			GameObject tmpDeadBody = Instantiate (deadSnake, (Vector2)listBody [i].transform.position+new Vector2(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f)),listBody [i].transform.rotation);
			NetworkServer.Spawn (tmpDeadBody);
		}
		player.GetComponent<UserControl> ().DestroyBody ();

	}
}
