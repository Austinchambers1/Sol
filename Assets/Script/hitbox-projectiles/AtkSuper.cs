using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkSuper : AtkDash {

	public GameObject multiHitbox;
	public float multiHitDuration;
	public float multiHitInterval;
	public Vector2 minKnockback;
	public Vector2 maxKnockback;
	public List<Attackable> collidedObjs = new List<Attackable> (); 

	// Use this for initialization
	void Start () {
		Vector3 newPos = new Vector3(transform.position.x, transform.position.y, 0);
	}

	
	// Update is called once per frame
	void Update () {}
	public override void onStartUp() {
		base.onStartUp ();
		collidedObjs.Clear ();
	}

	public override void onHitConfirm(GameObject other) {
		if (!collidedObjs.Contains (other.GetComponent<Attackable> ())) {
			
			Debug.Log ("creating multihit");
			GameObject mH = Instantiate (multiHitbox, other.transform.position, Quaternion.identity);
			hitboxMulti newBox = mH.GetComponent<hitboxMulti> ();
			newBox.setDamage (damage);
			newBox.setHitboxDuration (multiHitDuration);
			newBox.randomizeKnockback (minKnockback.x, maxKnockback.x, minKnockback.x, maxKnockback.y);
			newBox.setFaction (GetComponent<Attackable> ().faction);
			newBox.setFollow (other, Vector2.zero);
			newBox.creator = gameObject;
			newBox.reflect = false;
			newBox.stun = stun;
			newBox.refreshTime = multiHitInterval;
			newBox.randomizeKnockback (minKnockback.x, maxKnockback.x, minKnockback.y, maxKnockback.y);
			collidedObjs.Add (other.GetComponent<Attackable> ());
		}
	}
}
