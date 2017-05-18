using UnityEngine;
using System.Collections.Generic;

public class hitbox : MonoBehaviour {

	public List<Attackable> collidedObjs = new List<Attackable> (); 
	public float damage = 10.0f;
	public bool fixedKnockback = false;
	public Vector2 knockback = new Vector2(0.0f,40.0f);
	public float hitboxDuration = 1.0f;
	public bool timedHitbox = true;
	public string faction = "noFaction";
	public GameObject followObj;
	public GameObject creator;
	public GameObject hitFX;
	public bool toFollow = false;
	public bool reflect = true;
	public Vector2 followOffset;
	public float stun = 0.0f;
	public List<string> mAttr;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (hitboxDuration > 0.0f && timedHitbox) {
			hitboxDuration = hitboxDuration - Time.deltaTime;
		} else {
			GameObject.Destroy (gameObject);
		}
		if (followObj != null && toFollow) {
			transform.position = new Vector3(followObj.transform.position.x + followOffset.x, followObj.transform.position.y + followOffset.y,0);
		}
	}
	public void setKnockback(Vector2 kb) {
		knockback = kb;
	}

	public void setFixedKnockback(bool fixedKB) {
		fixedKnockback = fixedKB;
	}

	public void setDamage(float dmg) {
		damage = dmg;
	}
	public void setHitboxDuration (float time) {
		hitboxDuration = time;
	}
	public void setScale(Vector2 scale) {
		transform.localScale = scale;
	}
	public void setFaction(string fact) {
		faction = fact;
	}
	public void setFollow(GameObject obj, Vector2 offset) {
		toFollow = true;
		followObj = obj;
		followOffset = offset;
	}
	public void addAttribute(string attr) {
		mAttr.Add (attr);
	}
	internal void OnTriggerEnter2D(Collider2D other)
	{
//		Debug.Log (other.gameObject.GetComponent<Attackable> () );
		if (reflect && other.gameObject.GetComponent<Projectile> () && creator) {
			creator.GetComponent<HitboxMaker> ().registerHit (other.gameObject);
		}
		if (other.gameObject.GetComponent<Attackable>()){
			//Debug.Log("mFact: " + faction + " tFact: " + other.gameObject.GetComponent<Attackable>().faction);
		}
		if (other.gameObject.GetComponent<Attackable>() &&
			!collidedObjs.Contains (other.gameObject.GetComponent<Attackable> ())) {
			Attackable otherObj = other.gameObject.GetComponent<Attackable> ();
			if (faction == "noFaction" || otherObj.faction == "noFaction" ||
			    faction != otherObj.faction) {
				otherObj.takeHit (this);
				/*Vector3 newPos = new Vector3 ((other.transform.position.x + transform.position.x)/2f,
					(other.transform.position.y + transform.position.y)/2f,
						(other.transform.position.z + transform.position.z)/2f);*/
				
				GameObject fx = GameObject.Instantiate (hitFX, other.gameObject.transform.position,Quaternion.identity);
				fx.GetComponent<Follow> ().followObj = other.gameObject;
				float angle = (Mathf.Atan2 (knockback.y, knockback.x) * 180 )/ Mathf.PI;
//				Debug.Log (angle);
				fx.transform.Rotate(new Vector3(0f,0f, angle));
				if (creator) {
					//Debug.Log ("Damage confirm");
					if (creator.GetComponent<HitboxMaker> ()) {
						creator.GetComponent<HitboxMaker> ().registerHit (other.gameObject);
					} else if (creator.GetComponent<Shooter> ()) {
						creator.GetComponent<Shooter> ().registerHit (other.gameObject);
					}
				}
			}
			collidedObjs.Add (otherObj);
		}
	}
}
