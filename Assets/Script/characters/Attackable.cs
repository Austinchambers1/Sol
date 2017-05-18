using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour {

	public float bottomOfTheWorld = -10.0f;
	public float health = 100.0f;
	public float max_health = 100.0f;
	public float energy = 100.0f;
	public float max_energy = 100.0f;
	public bool alive = true;
	public bool immortal = false;
	public string faction = "noFaction";
	public GameObject HitEffect;
	public GameObject HealEffect;
	public GameObject DeathEffect;
	public float EnergyRegenRate = 10.0f;
	public string mHitSound = "None";
	public float deathTime = 0.0f;
	public Color deathColor = new Color(0.0f,0.0f,0.0f);
	float currDeathTime;
	SpriteRenderer renderer;


	public Dictionary<string,float> resistences = new Dictionary<string,float>();

	Movement movementController;
	// Use this for initialization
	void Start () {
		movementController = gameObject.GetComponent<Movement> ();
		health = Mathf.Min (health, max_health);
		currDeathTime = deathTime;
		renderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		alive = transform.position.y >= bottomOfTheWorld && health > 0;
		if (!alive && !immortal) {
			if (currDeathTime > 0.0) {
				if (currDeathTime == deathTime) {
					Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z - 3);
					GameObject deathfx = GameObject.Instantiate (DeathEffect, pos, Quaternion.identity);
					deathfx.GetComponent<disappearing> ().duration = deathTime + 1.0f;
					deathfx.GetComponent<Follow> ().followObj = gameObject;
					deathfx.GetComponent<Follow> ().followOffset = new Vector3 (0.0f, 0.0f, -3.0f);
					deathfx.GetComponent<Follow> ().toFollow = true;
					ParticleSystem [] partsys = deathfx.GetComponentsInChildren<ParticleSystem> ();
					foreach (ParticleSystem p in partsys) {
						ParticleSystem.MainModule mainP = p.main;
						mainP.startColor = deathColor;
						mainP.startLifetime = deathTime + 1.0f;
					}
				}
				renderer.color = Color.Lerp (Color.white, deathColor, (deathTime - currDeathTime) / deathTime);
				currDeathTime -= Time.deltaTime;
			} else {
				if (gameObject.name.Contains ("Enemy")) {
					FindObjectOfType<GameManager> ().soundfx.transform.FindChild ("EnemyDeath").GetComponent<AudioSource> ().Play ();
				} else if (gameObject.name.Contains ("Giant")) {
					FindObjectOfType<GameManager> ().soundfx.transform.FindChild ("GiantDeath").GetComponent<AudioSource> ().Play ();
				}
				Destroy (gameObject);
			}
		}
		modifyEnergy(EnergyRegenRate * Time.deltaTime);
		Dictionary<string,float>.KeyCollection keys = resistences.Keys;
		ArrayList toRemove = new ArrayList();
		foreach (string k in keys) {
			//Debug.Log ("key: " + k + " time: " + resistences [k]);
			float time = resistences [k] - Time.deltaTime;
			resistences [k] = time;
			if (resistences [k] <= 0.0f) {
				//Debug.Log ("removing resistance for" + k);
//				resistences.Remove (k);

				// BUG! You can't modify resistences while you're iterating through it!
				toRemove.Add (k);
			}
		}
		foreach (string k in toRemove) {
			resistences.Remove(k);
		}
	}

	public void addResistence(string attribute, float time) {
		/*if (resistences.ContainsKey(attribute)) {
			resistences.Remove (attribute);
		}
		resistences.Add (attribute, time);*/
		resistences [attribute] = time;
	}

	public string takeHit(hitbox hb) {
		
		if (hb.mAttr != null) {
			foreach (string k in resistences.Keys) {
				Debug.Log (k);
				if (hb.mAttr.BinarySearch(k) != null) {
					if (hb.stun > 0 && GetComponent<Fighter> ()) {
						GetComponent<Fighter> ().registerStun( hb.stun,false,hb);
					}
					Debug.Log ("attack blocked");
					return "block";
				}
			}
		}
		damageObj (hb.damage);
		if (mHitSound != "None") {
			FindObjectOfType<GameManager> ().soundfx.gameObject.transform.FindChild (mHitSound).GetComponent<AudioSource> ().Play ();
		}
		if (gameObject.GetComponent<Movement> ()) {
			if (hb.fixedKnockback) {
				addToVelocity (hb.knockback);
			} else {
				Vector3 otherPos = hb.gameObject.transform.position;
				float angle = Mathf.Atan2 (transform.position.y - otherPos.y, transform.position.x - otherPos.x); //*180.0f / Mathf.PI;
				float magnitude = hb.knockback.magnitude;
				float forceX = Mathf.Cos (angle) * magnitude;
				float forceY = Mathf.Sin (angle) * magnitude;
				Vector2 force = new Vector2 (forceX, forceY);
				float counterF = (gameObject.GetComponent<Movement> ().velocity.y * (1 / Time.deltaTime));
				if (counterF < 0) {
					force.y = force.y - counterF;
				}
				addToVelocity (force);
			}
		}
		if (hb.stun > 0 && GetComponent<Fighter> ()) {
			GetComponent<Fighter> ().registerStun( hb.stun,true,hb);
		}
		return "hit";
	}
	public void damageObj(float damage) {
		//Debug.Log ("Damage Taken. Health before: " + health);
		health = Mathf.Max(Mathf.Min(max_health, health - damage),0);
		if (damage > 0) {
			GameObject.Instantiate (HitEffect, transform.position, Quaternion.identity);
		} else if (damage < 0) {
			GameObject.Instantiate (HealEffect, transform.position, Quaternion.identity);
		}
		//Debug.Log("Health afterwards: " + health);
		if (health < 0) {
			alive = false;
		} else {
			alive = true;
		}
	}

	public void modifyEnergy(float energyDiff) {
		//Debug.Log ("Damage Taken. Health before: " + health);
		energy = Mathf.Max(Mathf.Min(max_energy, energy + energyDiff),0);
		if (energyDiff > 20) {
			GameObject.Instantiate (HealEffect, transform.position, Quaternion.identity);
		}
		//Debug.Log("Health afterwards: " + health);
	}
	public void resetHealth() {
		damageObj (-1000f);
	}

	public void addToVelocity(Vector2 veloc )
	{
		if (movementController) {
			movementController.addToVelocity(veloc);
		} 
	}

	public void AddConstantVel(Vector2 veloc, float time)
	{
		if (movementController) {
			movementController.addSelfForce (veloc, time);
		}
	}
}
