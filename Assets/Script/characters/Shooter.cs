using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {

	public GameObject projectile;
	// Use this for initialization
	void Start () {}
	// Update is called once per frame
	void Update () {}

	public Projectile fire(Vector2 offset,bool facingLeft,string faction) {
		Vector3 newPos = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, 0);
		GameObject go = Instantiate(projectile,newPos,Quaternion.identity) as GameObject; 
		Projectile proj = go.GetComponent<Projectile> ();
		hitbox newBox = go.GetComponent<hitbox> ();
		newBox.setFaction (faction);
		if (facingLeft) {
			SpriteRenderer sprite = go.GetComponent<SpriteRenderer> ();
			sprite.flipX = true;
			proj.projectileSpeed = new Vector3 (-proj.projectileSpeed.x, proj.projectileSpeed.y, 0f);
		}
		return proj;
	}
}
