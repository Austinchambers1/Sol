using System;
using UnityEngine;

public class AttackInfo : MonoBehaviour {
	public float damage = 10.0f;
	public Vector2 knockback = new Vector2(0.0f,40.0f);
	public Vector2 hitboxScale = new Vector2 (1.0f, 1.0f);
	public float hitboxDuration = 0.5f;
	public float stun = 0.3f;
	public Vector2 offset = new Vector2(0f,0f);
	public float startUpTime = 0.0f;
	public float recoveryTime = 1.0f;
	public int animationID = 1;
	public int recoveryAnimID = -1;
	public float animSpeed = 1f;
	public AudioSource soundFX;
	public string attackName = "default";
	public float timeSinceStart = 0.0f;
	public bool melee = true;
	public bool createHitbox = true;
	protected Fighter fighter;
	protected Attackable attackable;

	void Start () {
		fighter = GetComponent<Fighter> ();
		attackable = GetComponent<Attackable> ();
		if (recoveryAnimID <= 0) {
			recoveryAnimID = animationID;
		}
	}

	// Update is called once per frame
	void Update () {}

	public virtual void onStartUp() {}

	public virtual void onAttack() {}

	public virtual void onConclude() {
	}
	public virtual void onHitConfirm(GameObject other) {
	}
	public virtual void onInterrupt(float stunTime, bool successfulHit, hitbox hb) {
	}
}

