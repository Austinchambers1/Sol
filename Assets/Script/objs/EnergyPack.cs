using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPack : MonoBehaviour {
	public float energyValue = 15.0f;
	Movement movement;
	Vector3 velocity; 
	// Use this for initialization
	void Start () {
		movement = gameObject.GetComponent<Movement> ();
		velocity = new Vector2 ();
	}

	// Update is called once per frame
	void Update () {
		if (movement.collisions.above || movement.collisions.below) {
			velocity.y = 0.0f;
		}
		velocity.y += -movement.gravityScale * Time.deltaTime;
		movement.Move (velocity,Vector2.zero);
	}

	internal void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<Player> ()) {
			other.gameObject.GetComponent<Attackable> ().modifyEnergy (energyValue);
			GameObject.Destroy (gameObject);
			FindObjectOfType<GameManager> ().soundfx.gameObject.transform.FindChild ("PowerUp").GetComponent<AudioSource> ().Play ();
		}
	}
}
