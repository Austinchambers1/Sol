using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public Vector3 projectileSpeed;
	public float rotationSpeed = 90.0f;
	public bool rotating = false;
	public bool penetrating = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += projectileSpeed * Time.deltaTime;
		if (rotating) {
			transform.Rotate (Vector3.forward * (rotationSpeed * Time.deltaTime));
		}
	}
	internal void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<Attackable> () && other.gameObject.GetComponent<Attackable> ().faction == GetComponent<hitbox> ().faction ||
			penetrating) {
		} else {
			Destroy (gameObject);
		}
	}
}
