using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Movement))]
[RequireComponent (typeof (Fighter))]
[RequireComponent (typeof (Attackable))]
public class Player : MonoBehaviour {

	public Vector2 startPosition = new Vector2 (-4.0f, -3f);
	public float jumpHeight = 4.0f;
	public float timeToJumpApex = .4f;
	public string playerName = "Player 1";
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 8.0f;

	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	public string leftKey = "a";
	public string rightKey = "d";
	public string upKey = "space";
	public string downKey = "s";
	public string jumpKey = "w";
	public bool spawnNextToEndzone = false;

	public bool attemptingInteraction = false;
	Movement controller;
	Attackable attackable;
	public bool canDoubleJump = true;

	float timeSinceLeft = 0.0f;
	float timeSinceRight = 0.0f;
	float timeSinceLastDash = 0.0f;
	public float dashSpeed = 20f;
	float timeSinceLastAttack = 0.0f;
	public float dashTime = 0.15f;
	public float P1AbilityCost = 20.0f;
	public float inputX = 0.0f;
	public float inputY = 0.0f;

	public float dashCooldown = 0.5f;

	public bool grounded;

	private GameManager gameManager;

	Animator anim;
	public float lastHealth;
	internal void Start() {
		anim = GetComponent<Animator> ();
		controller = GetComponent<Movement> ();
		attackable = GetComponent<Attackable> ();
		Reset ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		controller.setGravityScale(gravity);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		gameManager = FindObjectOfType<GameManager> ();
		lastHealth = GetComponent<Attackable> ().health;
	}

	public void Reset() {
		if (spawnNextToEndzone)
			startPosition = new Vector2 (105f, 14f); // this is right next to the endzone. (in Scene.unity)
		//else
			//startPosition = new Vector2 (-4.0f, -3f);
		transform.position = startPosition;
		controller.accumulatedVelocity = Vector2.zero;
		attackable.resetHealth ();
		FindObjectOfType<PlayerCursor> ().currentPower = 20.0f;
		attackable.energy = 20.0f;
		// reset should also bring back the startblock, if we want to keep using it.

	}

	internal void Update() {
		timeSinceLeft += Time.deltaTime;
		timeSinceRight += Time.deltaTime;
		timeSinceLastDash += Time.deltaTime;
		timeSinceLastAttack += Time.deltaTime;
		anim.SetBool ("grounded", controller.onGround);
		anim.SetBool ("tryingToMove", false);
		if (timeSinceLastAttack > 0.3f) {
			anim.SetBool ("isattacking", false);
		}
		//if (lastHealth > GetComponent<Attackable> ().health) {
		//			Debug.Log ("Reset");
		//	FindObjectOfType<PlayerCursor> ().timeSinceLastHit = 0.0f;
		//}
		lastHealth = GetComponent<Attackable> ().health;
		if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && attackable.energy >= P1AbilityCost && timeSinceLastDash > dashCooldown) {
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
		if (Input.GetKey (leftKey) ) {
			timeSinceLeft = 0.0f;
		}

		if (Input.GetKey (rightKey)) {
			timeSinceRight = 0.0f;
		}

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0.0f;
		}
		if (controller.onGround) {
			canDoubleJump = true;
		}

		inputX = 0.0f;
		inputY = 0.0f;
		if (Input.GetKey(leftKey)) { 
			anim.SetBool ("tryingToMove",true);
			controller.setFacingLeft (true);
			inputX = -1.0f; 
		}  
		else if (Input.GetKey(rightKey)) { 
			anim.SetBool ("tryingToMove",true);
			inputX = 1.0f; 
			controller.setFacingLeft (false);
		}

		if (Input.GetKey(upKey)) { inputY = 1.0f; } 
		else if (Input.GetKey(downKey) ){ inputY = -1.0f; }

		if (Input.GetKeyDown (downKey)) {
			if (gameObject.GetComponent<Fighter> ().tryAttack ()) {
				timeSinceLastAttack = 0.0f;
				anim.SetBool ("isattacking", true);
				gameManager.soundfx.gameObject.transform.FindChild ("P1Attack").GetComponent<AudioSource> ().Play ();
			}
			attemptingInteraction = true;
		} else {
			attemptingInteraction = false;
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

		float targetVelocityX = inputX * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		Vector2 input = new Vector2 (inputX, inputY);
		velocity.y += gravity * Time.deltaTime;
		//Debug.Log (gravity);
		controller.Move (velocity, input);

		if (!attackable.alive) {
			gameManager.gameOver = true;
			gameManager.winner = 2;
			Reset ();
		}
			
	}
}
