using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Fighter))]
public class SimpleBeatAtk : Beats {

	public string attackName = "attack";
	public int attackOnBeat = 1;
	Fighter mFighter;
	// Use this for initialization
	void Start () {
		base.init ();
		mFighter = GetComponent<Fighter> ();
	}
	void OnDestroy() {
		base.dest ();
	}

	// Update is called once per frame
	void Update () {}

	public override void onBeat (int beatNo) {
		//Debug.Log ("on simple beat");
		if (beatNo == attackOnBeat) {
			//Debug.Log ("trying attack: " + attackName);
			mFighter.tryAttack (attackName);
		}
	}
}
