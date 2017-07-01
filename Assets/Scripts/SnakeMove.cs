using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class SnakeMove : NetworkBehaviour {
	
	public float speed;
	public GameObject gBody;

	protected List<Vector2> path = new List<Vector2>();
	protected List<GameObject> lstBody = new List<GameObject>();
	protected int FPS = 50;

	public abstract void Destroy ();
	public abstract List<GameObject> GetBody ();
	public abstract void addScore (int score);
}
