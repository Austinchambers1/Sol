﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkReflector : AttackInfo {

	public bool rapidRecovery = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public override void onStartUp() {
		GetComponent<Fighter> ().reflectProj = true;
	}
	public override void onHitConfirm(GameObject other) {
		Debug.Log ("hitConfirm");
		if (other.GetComponent<Projectile> ()) {
			Projectile p = other.GetComponent<Projectile> ();
			//other.GetComponent<Movement> ().facingLeft = !other.GetComponent<Movement> ().facingLeft;
			p.projectileSpeed.x = p.projectileSpeed.x * -1f;
			p.projectileSpeed.y = p.projectileSpeed.y * -1f;
			hitbox hb = other.GetComponentInChildren<hitbox> ();
			hb.creator = gameObject;
			//Debug.Log ("old: " + hb.faction);
			hb.faction = GetComponent<Attackable> ().faction;
			hb.collidedObjs = new List<Attackable> (); 
			//Debug.Log ("new: " + hb.faction);
			hb.hitboxDuration = Mathf.Min(Mathf.Max(0.5f,hb.hitboxDuration),2.0f) * 4.0f;
			p.duration = hb.hitboxDuration;
			if (rapidRecovery) {
				GetComponent<Fighter>().recoveryTime = GetComponent<Fighter>().recoveryTime * 0.3f;
			}
		}
	}
}
