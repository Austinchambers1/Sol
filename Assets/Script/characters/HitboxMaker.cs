﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxMaker : MonoBehaviour {

	public GameObject hitboxClass;
	public bool hitboxReflect = false;
	public float stun = 0f;
	public List<string> mAttrs;
	void Start () {}
	void Update() {}
	public hitbox createHitbox(Vector2 hitboxScale, Vector2 offset,float damage, float hitboxDuration, Vector2 knockback,bool fixedKnockback,string faction, bool followObj) {
		Vector3 newPos = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, 0);
		GameObject go = Instantiate(hitboxClass,newPos,Quaternion.identity) as GameObject; 
		hitbox newBox = go.GetComponent<hitbox> ();
		newBox.setScale (hitboxScale);
		newBox.setDamage (damage);
		newBox.setHitboxDuration (hitboxDuration);
		newBox.setKnockback (knockback);
		newBox.setFixedKnockback (fixedKnockback);
		newBox.setFaction (faction);
		newBox.creator = gameObject;
		newBox.reflect = hitboxReflect;
		newBox.stun = stun;
		newBox.mAttr = mAttrs;
		if (followObj) {
			newBox.setFollow (gameObject,offset);
		}
		return newBox;
	}
	public void registerHit(GameObject otherObj) {
		if (gameObject.GetComponent<Fighter> ()) {
			gameObject.GetComponent<Fighter> ().registerHit (otherObj);
		}
		if (gameObject.GetComponent<Shooter> ()) {
			gameObject.GetComponent<Shooter> ().registerHit (otherObj);
		}
	}
	public void addAttrs(string attr) {
		mAttrs.Add (attr);
	}

	public void clearAttrs() {
		mAttrs = new List<string> ();
	}
}
