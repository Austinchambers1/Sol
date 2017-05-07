using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (FollowPlayer))]
public class AttackIfClose : MonoBehaviour {

	public float attackDist = 0.5f;
	public string attackName = "attack";
	FollowPlayer followai;
	Movement movt;
	// Use this for initialization
	void Start () {
		followai = GetComponent<FollowPlayer> ();
		movt = GetComponent<Movement> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (followai.targetSet && movt.canMove) {	
			if (Vector3.Distance (followai.followObj.transform.position, transform.position) < attackDist) {
				gameObject.GetComponent<Fighter> ().tryAttack (attackName);
			}
		}
	}
}
