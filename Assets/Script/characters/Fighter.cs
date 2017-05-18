﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (HitboxMaker))]
[RequireComponent (typeof (Movement))]

public class Fighter : MonoBehaviour {
	public float recoveryTime = 0.0f;
	public float startUpTime = 0.0f;
	public float stunTime = 0.0f;
	public Dictionary<string,AttackInfo> attacks = new Dictionary<string,AttackInfo>();
	string myFac;
	Movement movement;
	Animator anim;
	Attackable attackable;
	GameManager gameManager;
	HitboxMaker hbm;
	AttackInfo currentAttack;
	public string currentAttackName;
	bool hitboxCreated;
	public bool reflectProj;
	float maxStun;

	float beatTime;
	float animationRatio;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		movement = GetComponent<Movement> ();
		gameManager = FindObjectOfType<GameManager> ();
		attackable = GetComponent<Attackable> ();
		myFac = gameObject.GetComponent<Attackable> ().faction;
		hbm = GetComponent<HitboxMaker> ();
		endAttack ();
		AttackInfo[] at = gameObject.GetComponents<AttackInfo> ();
		foreach (AttackInfo a in at) {
			attacks.Add (a.attackName, a);
		}
		beatTime = 60.0f / FindObjectOfType<BeatTracker> ().Tempo;
		animationRatio = FindObjectOfType<BeatTracker> ().Tempo / 100.0f;
	}

	// Update is called once per frame
	void Update () {
		anim.SetBool ("tryingToMove", false);
		anim.SetBool ("grounded", movement.collisions.below);
		if (GetComponent<FollowPlayer> ()) {
			if (GetComponent<FollowPlayer> ().inputX != 0.0f) {
				anim.SetBool ("tryingToMove", true);
			}
		} else if (GetComponent<Player> ()) {
			if (GetComponent<Player> ().inputX != 0.0f) {
				anim.SetBool ("tryingToMove", true);
			}
		}
		if (stunTime > 0.0f ) {
			if (stunTime != maxStun) {
				anim.SetBool ("hitInit", false);
			}
			stunTime = Mathf.Max (0.0f, stunTime - Time.deltaTime);
			if (stunTime == 0.0f && attackable.alive) {
				endStun ();
			}
		}else if (!attackable.alive) {
			startHitState (10.0f);
		}
		if (currentAttackName != "none") {
			currentAttack.timeSinceStart = currentAttack.timeSinceStart + Time.deltaTime;
			if (hitboxCreated == false) {
				if (startUpTime <= (Time.deltaTime/2)) {
					hitboxCreated = true;
					anim.SetInteger ("attack", currentAttack.animationID);
					currentAttack.onAttack ();
					if (currentAttack.soundFX != null) {
						currentAttack.soundFX.Play ();
					}
					if (currentAttack.attackFX) {
						Debug.Log ("Creating effect");
						GameObject fx = GameObject.Instantiate (currentAttack.attackFX, transform);
						fx.GetComponent<disappearing> ().duration = currentAttack.recoveryTime;

						fx.GetComponent<disappearing> ().toDisappear = true;
						fx.GetComponent<Follow> ().followObj = gameObject;
						fx.GetComponent<Follow> ().followOffset = new Vector3 (0.0f, 0.0f, -3.0f);
						fx.GetComponent<Follow> ().toFollow = true;
						if (movement.facingLeft) {
							fx.transform.Rotate (new Vector3 (0f, 180f,0f));
						}

						ParticleSystem [] partsys = fx.GetComponentsInChildren<ParticleSystem> ();
						foreach (ParticleSystem p in partsys) {
							ParticleSystem.MainModule mainP = p.main;
							//mainP.startColor = currentAttack.attackPrimaryColor;
							mainP.startLifetime = currentAttack.recoveryTime + 0.2f;
						}
					}
					if (currentAttack.createHitbox) {
						Vector2 realKB = currentAttack.knockback;
						Vector2 realOff = currentAttack.offset;
						if (currentAttack.melee) {
							hbm.addAttrs ("melee");
						}
						if (gameObject.GetComponent<Movement> ().facingLeft) {
							realKB = new Vector2 (-currentAttack.knockback.x, currentAttack.knockback.y);
							realOff = new Vector2 (-currentAttack.offset.x, currentAttack.offset.y);
						}
						hbm.hitboxReflect = reflectProj;
						hbm.stun = currentAttack.stun;
						hbm.createHitbox (currentAttack.hitboxScale, realOff, currentAttack.damage, currentAttack.hitboxDuration, realKB, true, myFac, true);
					}
					if (currentAttack.recoveryAnimID != currentAttack.animationID && currentAttack.recoveryAnimID > 0) {
						anim.SetInteger ("attack", currentAttack.recoveryAnimID);
					}
				} else {
					startUpTime = Mathf.Max (0.0f, startUpTime - Time.deltaTime);
				}
			} else {
				if (recoveryTime <= Time.deltaTime/2.0f) {
					endAttack ();

				} else {
					recoveryTime = Mathf.Max (0.0f, recoveryTime - Time.deltaTime);
				}
			}
		}
	}
	public bool isAttacking() {
		return (currentAttackName == "none");
	}

	public void registerStun(float st, bool defaultStun,hitbox hb) {
		if (defaultStun) {
			startHitState (st);
		}
		if (currentAttack != null) {
			currentAttack.onInterrupt (stunTime,defaultStun,hb);
		}
	}
	void startHitState(float st) {
		anim.SetBool ("hit", true);
		anim.SetBool ("hitInit", true);
		endAttack ();
		stunTime = st;
		maxStun = st;
		movement.canMove = false;
	}
	public void registerHit(GameObject otherObj) {
		if (currentAttack != null) {
			currentAttack.onHitConfirm (otherObj);
		}
	}

	public void endStun() {
		if (attackable.alive) {
			anim.SetBool ("hit", false);
			anim.SetBool ("hitInit", false);
			movement.canMove = true;
			hbm.clearAttrs ();
			stunTime = 0.0f;
			maxStun = 0.0f;
		}
	}
	public void endAttack() {
		if (currentAttack != null) {
			currentAttack.onConclude ();
			currentAttack.timeSinceStart = 0.0f;
		}
		currentAttackName = "none";
		startUpTime = 0.0f;
		recoveryTime = 0.0f;
		anim.speed = 1.0f;
		hitboxCreated = false;
		currentAttack = null;
		reflectProj = false;
		anim.SetInteger ("attack", 0);
		movement.canMove = true;
	}
	public bool tryAttack(string attackName) {
		if (currentAttackName == "none" && attacks.ContainsKey(attackName)) {
			hitboxCreated = false;
			currentAttackName = attackName;
			currentAttack = attacks[currentAttackName];
			startUpTime = (currentAttack.startUpTime * beatTime) - (Time.deltaTime * 2);
			recoveryTime = currentAttack.recoveryTime * beatTime;
			anim.SetInteger ("attack", currentAttack.animationID);
			anim.speed = currentAttack.animSpeed * animationRatio;
			movement.canMove = false;
			currentAttack.onStartUp ();
			currentAttack.timeSinceStart = 0.0f;
			return true;
		}
		return false;
	}
}
