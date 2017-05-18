using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Movement))]
[RequireComponent (typeof (Fighter))]
[RequireComponent (typeof (Attackable))]
public class Player : MonoBehaviour {

	// Movement 
	public Vector2 startPosition = new Vector2 (-4.0f, -3f);
	public float jumpHeight = 4.0f;
	public float timeToJumpApex = .4f;
	public string playerName = "Player 1";
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 8.0f;

	float gravity;
	float jumpVelocity;
	Vector2 velocity;
	float velocityXSmoothing;

	//controls
	public string leftKey = "a";
	public string rightKey = "d";
	public string upKey = "w";
	public string downKey = "s";
	public string jumpKey = "w";

	public string attackKey = "j";
	public string reflectKey = "l";
	public string guardKey = "k";
	//-------------------

	bool spawnNextToEndzone = false;

	public bool attemptingInteraction = false;
	Movement controller;
	Attackable attackable;
	Animator anim;
	GameManager gameManager;

	public bool canDoubleJump = true;

	public float dashTime = 0.15f;
	public float dashCooldown = 0.5f;
	public float P1AbilityCost = 20.0f;
	public float inputX = 0.0f;
	public float inputY = 0.0f;
	public float dashSpeed = 20f;
	float timeSinceLastDash = 0.0f;

	//public float lastHealth;

	internal void Start() {
		anim = GetComponent<Animator> ();
		controller = GetComponent<Movement> ();
		attackable = GetComponent<Attackable> ();
		Fighter f = GetComponent<Fighter> ();
		Reset ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		controller.setGravityScale(gravity);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		gameManager = FindObjectOfType<GameManager> ();
		//lastHealth = GetComponent<Attackable> ().health;		

	}

	public void Reset() {
		if (spawnNextToEndzone)
			startPosition = new Vector2 (105f, 14f); // this is right next to the endzone. (in Scene.unity)
		transform.position = startPosition;
		controller.accumulatedVelocity = Vector2.zero;
		attackable.resetHealth ();
		attackable.energy = 20.0f;
		// reset should also bring back the startblock, if we want to keep using it.
	}

	internal void Update() {
		timeSinceLastDash += Time.deltaTime;
		anim.SetBool ("grounded", controller.onGround);
		anim.SetBool ("tryingToMove", false);
		/*
		if (lastHealth > GetComponent<Attackable> ().health) {
					Debug.Log ("Reset");
			FindObjectOfType<PlayerCursor> ().timeSinceLastHit = 0.0f;
		}
		lastHealth = GetComponent<Attackable> ().health;*/
		if (controller.canMove && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && attackable.energy >= P1AbilityCost && timeSinceLastDash > dashCooldown) {
			float dash = dashSpeed;
			if (Input.GetKey (leftKey)) {
				dash = -dash;
			} else if (controller.facingLeft) {
				dash = -dash;
			}
			controller.addSelfForce (new Vector2 (dash, 0.0f),dashTime);
			attackable.modifyEnergy( -P1AbilityCost);
			timeSinceLastDash = 0.0f;
			gameManager.soundfx.gameObject.transform.FindChild ("P1Dash").GetComponent<AudioSource> ().Play ();
		}

		if (controller.collisions.above || controller.collisions.below) {velocity.y = 0.0f;}
		if (controller.onGround) {canDoubleJump = true;}

		inputX = 0.0f;
		inputY = 0.0f;

		if (controller.canMove) {
			if (Input.GetKey (upKey)) {
				inputY = 1.0f;
			} else if (Input.GetKey (downKey)) {
				inputY = -1.0f;
			}

			if (Input.GetKeyDown (downKey)) {
				attemptingInteraction = true;
			} else {
				attemptingInteraction = false;
			}
			//Movement controls
			if (Input.GetKey (leftKey)) { 
				anim.SetBool ("tryingToMove", true);
				controller.setFacingLeft (true);
				inputX = -1.0f; 
			} else if (Input.GetKey (rightKey)) { 
				anim.SetBool ("tryingToMove", true);
				inputX = 1.0f; 
				controller.setFacingLeft (false);
			}
			//Attack/Reflect/Guard Animations
			if (Input.GetKeyDown (attackKey)) {
				if (Input.GetKey (downKey)) {
					Debug.Log ("trying down");
					gameObject.GetComponent<Fighter> ().tryAttack ("down");
				} else if (Input.GetKey (upKey)) {
					gameObject.GetComponent<Fighter> ().tryAttack ("up");
				}else if (Input.GetKey (leftKey) || Input.GetKey (rightKey)) {
					gameObject.GetComponent<Fighter> ().tryAttack ("dash");
				} else {
					gameObject.GetComponent<Fighter> ().tryAttack ("attack");
				}
			}
			if (Input.GetKeyDown (reflectKey)) {
				gameObject.GetComponent<Fighter> ().tryAttack ("reflect");
			}
			if (Input.GetKeyDown (guardKey)) {
				gameObject.GetComponent<Fighter> ().tryAttack ("guard");
			}

			
			if (Input.GetKeyDown (jumpKey)) {
				if (controller.collisions.below) {
					velocity.y = jumpVelocity;
					gameManager.soundfx.gameObject.transform.FindChild ("P1Jump").GetComponent<AudioSource> ().Play ();
				} else if (canDoubleJump && attackable.energy >= P1AbilityCost) {
					velocity.y = jumpVelocity;
					gameManager.soundfx.gameObject.transform.FindChild ("P1Jump").GetComponent<AudioSource> ().Play ();
					canDoubleJump = false;
					attackable.energy = attackable.energy - P1AbilityCost;
				}
			}
		}

		//Movement logic
		float targetVelocityX = inputX * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		Vector2 input = new Vector2 (inputX, inputY);
		//velocity.y += gravity * Time.deltaTime;
		//Debug.Log("inPlayer: " + gravity * Time.deltaTime);
		Debug.Log("Player veloc: " + velocity);
		velocity = controller.Move (velocity, input);
		if (!attackable.alive) {
			//gameManager.gameOver = true;
			//gameManager.winner = 2;
			Reset ();
		}
			
	}
}
