using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Movement))]
public class FollowPlayer : MonoBehaviour {

	public Player followObj;
	public float bottomOfTheWorld = -10.0f;
	Movement controller;
	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	public float jumpHeight = 4.0f;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 8.0f;
	public bool targetSet = true;
	public float minDistance = 1.0f;
	public float maxDistance = 10.0f;
	public float inputX = 0.0f;
	float inputY = 0.0f;


	void Start () {
		controller = GetComponent<Movement> ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		controller.setGravityScale(gravity);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		setTarget(FindObjectOfType<Player> ());
	}

	void setTarget(Player target) {
		targetSet = true;
		followObj = target;
	}

	public void moveToPlayer() {
		inputX = 0.0f;
		inputY = 0.0f;
		float dist = Vector3.Distance (transform.position, followObj.transform.position);
		if (controller.canMove && dist < maxDistance) {

			if (followObj.transform.position.x > transform.position.x) {
				if ( dist > minDistance)
					inputX = 1.0f;
				controller.setFacingLeft (false);

			} else {
				if ( dist > minDistance)
					inputX = -1.0f;
				controller.setFacingLeft (true);
			}
		}
		float targetVelocityX = inputX * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		Vector2 input = new Vector2 (inputX, inputY);

		if (controller.canMove && (controller.falling == "left" || controller.falling == "right") && controller.collisions.below) {
			velocity.y = jumpVelocity;
		}
		velocity.y += gravity * Time.deltaTime;

		controller.Move (velocity, input);
	}

	// Update is called once per frame
	void Update () {
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0.0f;
		}
		moveToPlayer ();

	}
}
