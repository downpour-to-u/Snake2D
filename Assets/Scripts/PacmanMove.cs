using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMove : MonoBehaviour {

	public float speed = 0.4f;
	Vector2 dest = Vector2.zero;
	Rigidbody2D rb2;

	// Use this for initialization
	void Start () {
		dest =transform.position;
		rb2 = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		// Move closer to Destination
		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		rb2.MovePosition(p);
		Debug.Log ((Vector2)transform.position);
		// Check for Input if not moving
		if ((Vector2)transform.position == dest) {
			if (Input.GetKey(KeyCode.UpArrow) && valid(Vector2.up))
			{
				dest = (Vector2)transform.position + Vector2.up;
			}
			if (Input.GetKey (KeyCode.RightArrow) && valid (Vector2.right)) {
				dest = (Vector2)transform.position + Vector2.right;
				Debug.Log (dest);
			}
				
			if (Input.GetKey(KeyCode.DownArrow) && valid(-Vector2.up))
				dest = (Vector2)transform.position - Vector2.up;
			if (Input.GetKey(KeyCode.LeftArrow) && valid(-Vector2.right))
				dest = (Vector2)transform.position - Vector2.right;
		}

		// Animation Parameters
		Vector2 dir = dest - (Vector2)transform.position;
		GetComponent<Animator>().SetFloat("DirX", dir.x);
		GetComponent<Animator>().SetFloat("DirY", dir.y);
	}

	bool valid (Vector2 dir){
		// Cast Line from 'next to Pac-Man' to 'Pac-Man'
		Vector2 pos = transform.position;
		RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
		Debug.Log (hit.collider);
		return (hit.collider == GetComponent<Collider2D>());
	}

}
