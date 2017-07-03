using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GhostMove : SnakeMove {

	public Transform[] waypoints;
	int cur = 0;

	// Use this for initialization
	void Start () {
		for (float i = 10.0f; i > 0.0f; i -= 2.0f / FPS) {
			path.Add ((Vector2)transform.position - new Vector2 (i + 1, 0));
		}
		for (int i = 0; i < 9; i++) {
			GameObject tmpBody = Instantiate (gBody, (Vector2)transform.position - new Vector2 (i + 1, 0), new Quaternion ());
			lstBody.Add (tmpBody);
		}
	}

	void FixedUpdate () {
		// Waypoint not reached yet? then move closer
		if (transform.position != waypoints [cur].position) {
			Vector2 p = Vector2.MoveTowards (transform.position, waypoints [cur].position, speed/FPS);
			//add path
			path.Add (new Vector2 (p.x, p.y));
			//move head
			GetComponent<Rigidbody2D> ().MovePosition (p);
			//move bodies
			int lag = (int)Math.Round (FPS / speed);
			for (int i = 0; i < lstBody.Count; i++) {
				Vector2 tmpPosition = path [path.Count - lag - 1 - Math.Min (path.Count - lag - 1, i * lag)];
				lstBody[i].transform.position = new Vector2(tmpPosition.x,tmpPosition.y);
			}
			//remove unnecessary path node
			if (path.Count > (lstBody.Count + 1) * lag + lag * 2) {
				path.RemoveAt (0);
			}
		}
		// Waypoint reached, select next one
		else {
			cur = (cur + 1) % waypoints.Length;
		}

	}

	override public List<GameObject> GetBody(){
		return lstBody;
	}

	public override void addScore (int score)
	{
		//add body
		if (score / 5 > lstBody.Count - 9)
			addBody ();
	}

	public void addBody(){
		Vector2 forward = new Vector2((float)Math.Cos(transform.eulerAngles.z*Math.PI/180),(float)Math.Sin(transform.eulerAngles.z*Math.PI/180));
		GameObject tmpBody = Instantiate (gBody, (Vector2)transform.position - forward, transform.rotation);
		lstBody.Add (tmpBody);
	}
}
