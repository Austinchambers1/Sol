using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (HitboxMaker))]
//[RequireComponent (typeof (Shooter))]
[RequireComponent (typeof (Movement))]
public class Fighter : MonoBehaviour {
	public float damage = 10.0f;
	public Vector2 knockback = new Vector2(0.0f,40.0f);
	public Vector2 hitboxScale = new Vector2 (1.0f, 1.0f);
	public bool timedbomb = true;
	public float hitboxDuration = 0.5f;
	public float attackCooldown = 1.0f;
	public Vector2 offset = new Vector2(0f,0f);
	public bool canShoot;
	public GameObject projectile;
	float timeSinceLastAttack = 0.0f;

	float currentCooldown = 0.0f;
	string myFac;
	Movement controller;
	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		controller = GetComponent<Movement> ();
		myFac = gameObject.GetComponent<Attackable> ().faction;
		currentCooldown = attackCooldown / 2f;
	}
	
	// Update is called once per frame
	void Update () {
		anim.SetBool ("tryingToMove", false);

		anim.SetBool ("grounded", controller.collisions.below);
		if (GetComponent<FollowPlayer> ()) {
			if (GetComponent<FollowPlayer> ().inputX != 0.0f) {
				anim.SetBool ("tryingToMove", true);
			}
		} else if (GetComponent<Player> ()) {
			if (GetComponent<Player> ().inputX != 0.0f) {
				anim.SetBool ("tryingToMove", true);
			}
		}
		if (timeSinceLastAttack < 0.23f) {
			anim.SetBool ("isattacking", true);
		} else {
			anim.SetBool ("isattacking", false);
		}
		timeSinceLastAttack += Time.deltaTime;

		if (currentCooldown > 0.0f) {
			currentCooldown = currentCooldown - Time.deltaTime;
		}
	}

	public bool tryAttack() {
		if (currentCooldown <= 0.0f) {
			
			currentCooldown = attackCooldown;
			Vector2 realKB = knockback;
			Vector2 realOff = offset;
			timeSinceLastAttack = 0.0f;
			if (gameObject.GetComponent<Movement> ().facingLeft) {
				realKB = new Vector2 (-knockback.x, knockback.y);
				realOff = new Vector2 (-offset.x, offset.y);
			}
			gameObject.GetComponent<HitboxMaker> ().createHitbox(hitboxScale,realOff,damage,hitboxDuration,realKB,true,myFac,true);
			if (canShoot) {
				GetComponent<Shooter> ().fire(Vector2.zero,gameObject.GetComponent<Movement> ().facingLeft,myFac);
			}
			return true;
		}
		return false;
	}
}
