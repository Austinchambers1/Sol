using System.Collections;
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
	GameManager gameManager;
	AttackInfo currentAttack;
	public string currentAttackName;
	bool hitboxCreated;
	public bool reflectProj;
	float maxStun;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		movement = GetComponent<Movement> ();
		gameManager = FindObjectOfType<GameManager> ();
		myFac = gameObject.GetComponent<Attackable> ().faction;
		endAttack ();
		AttackInfo[] at = gameObject.GetComponents<AttackInfo> ();
		foreach (AttackInfo a in at) {
			attacks.Add (a.attackName, a);
		}
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
		if (stunTime > 0.0f) {
			if (stunTime != maxStun) {
				anim.SetBool ("hitInit", false);
			}
			stunTime = Mathf.Max (0.0f, stunTime - Time.deltaTime);
			if (stunTime == 0.0f) {
				endStun ();
			}
		}
		if (currentAttackName != "none") {
			currentAttack.timeSinceStart = currentAttack.timeSinceStart + Time.deltaTime;
			if (hitboxCreated == false) {
				if (startUpTime <= 0.0f) {
					hitboxCreated = true;
					if (currentAttack.createHitbox) {
						Vector2 realKB = currentAttack.knockback;
						Vector2 realOff = currentAttack.offset;
						currentAttack.onAttack ();
						if (gameObject.GetComponent<Movement> ().facingLeft) {
							realKB = new Vector2 (-currentAttack.knockback.x, currentAttack.knockback.y);
							realOff = new Vector2 (-currentAttack.offset.x, currentAttack.offset.y);
						}
						gameObject.GetComponent<HitboxMaker> ().hitboxReflect = reflectProj;
						gameObject.GetComponent<HitboxMaker> ().stun = currentAttack.stun;
						gameObject.GetComponent<HitboxMaker> ().createHitbox (currentAttack.hitboxScale, realOff, currentAttack.damage, currentAttack.hitboxDuration, realKB, true, myFac, true);
					}
				} else {
					startUpTime = Mathf.Max (0.0f, startUpTime - Time.deltaTime);
				}
			} else {
				if (recoveryTime <= 0.0f) {
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
			anim.SetBool ("hit", true);
			anim.SetBool ("hitInit", true);
			endAttack ();
			stunTime = st;
			maxStun = st;
			movement.canMove = false;

		}
		if (currentAttack != null) {
			currentAttack.onInterrupt (stunTime,defaultStun,hb);
		}
	}
	public void registerHit(GameObject otherObj) {
		if (currentAttack != null) {
			currentAttack.onHitConfirm (otherObj);
		}
	}

	public void endStun() {
		anim.SetBool ("hit", false);
		anim.SetBool ("hitInit", false);
		movement.canMove = true;
		stunTime = 0.0f;
		maxStun = 0.0f;
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
			startUpTime = currentAttack.startUpTime;
			recoveryTime = currentAttack.recoveryTime;
			anim.SetInteger ("attack", currentAttack.animationID);
			anim.speed = currentAttack.animSpeed;
			movement.canMove = false;
			currentAttack.onStartUp ();
			currentAttack.timeSinceStart = 0.0f;
			if (currentAttack.soundFX != "None") {
				gameManager.soundfx.gameObject.transform.FindChild (currentAttack.soundFX).GetComponent<AudioSource> ().Play ();
			}
			return true;
		}
		return false;
	}
}
