using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (FollowPlayer))]
public class AttackIfClose : Beats {

	public float attackDist = 0.5f;
	public string attackName = "attack";
	FollowPlayer followai;
	public int attackOnBeat = 1;
	bool inRange = false;

	Movement movt;
	// Use this for initialization
	void Start () {
		followai = GetComponent<FollowPlayer> ();
		movt = GetComponent<Movement> ();
		inRange = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (followai.targetSet && movt.canMove) {	
			if (Vector3.Distance (followai.followObj.transform.position, transform.position) < attackDist) {
				inRange = true;
			} else {
				inRange = false;
			}
		}
	}

	public override void onBeat (int beatNo) {
		//Debug.Log ("on simple beat");
		if (inRange && beatNo == attackOnBeat) {
			//Debug.Log ("trying attack: " + attackName);
			followai.moveToPlayer ();
			gameObject.GetComponent<Fighter> ().tryAttack (attackName);
		}
	}
}
