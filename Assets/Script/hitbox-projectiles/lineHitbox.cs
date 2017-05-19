using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LineRenderer))]
public class lineHitbox : hitbox {
	public float range = 1000;
	private LineRenderer line;
	public bool playerOnly = true;

	void Start ()
	{
		line = GetComponent<LineRenderer> ();
		line.SetVertexCount (2);
	}

	void Update () // consider void FixedUpdate()
	{
		base.updateTick ();
		RaycastHit2D hit;
		if (creator.GetComponent<Movement>().facingLeft) {
			hit = Physics2D.Raycast (transform.position, Vector2.left, range); // transform.position + (transform.right * (float)offset) can be used for casting not from center.
		} else {
			hit = Physics2D.Raycast (transform.position, Vector2.right, range); // transform.position + (transform.right * (float)offset) can be used for casting not from center.
		}

		if (hit) {
			line.SetPosition (0, transform.position);
			line.SetPosition (1, hit.point);
			Collider2D collider = hit.collider;
			OnTriggerEnter2D (collider);
		} else {
			line.SetPosition (0, transform.position);
			if (creator.GetComponent<Movement>().facingLeft) {
				line.SetPosition (1, transform.position + (Vector3.left * range)); // (transform.right * ((float)offset + range)) can be used for casting not from center.
			} else {
				line.SetPosition (1, transform.position + (transform.right * range)); // (transform.right * ((float)offset + range)) can be used for casting not from center.
			}
		}
	}

	public void setRange(float r) {
		range = r;
	}
}