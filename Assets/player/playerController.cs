using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {
	//movement
	public float maxSpeed = 10f;
	bool grounded = false;
	public bool jump = false;
	public Transform groundCheck;
	public LayerMask whatIsGround;
	public float jumpForce = 700f;
	
	float hInput = 0;
	
	//swipe input values
	public float minSwipeDistY = 300;
	public float minSwipeDistX = 300;
	private Vector2 startPos;

	void Update() {
		#if UNITY_ANDROID
		if (Input.touchCount > 0) {
			Touch touch = Input.touches [0];
			switch (touch.phase) {
			case TouchPhase.Began:
				startPos = touch.position;
				break;
			case TouchPhase.Ended:
				float swipeDistVertical = (new Vector3 (0, touch.position.y, 0) - new Vector3 (0, startPos.y, 0)).magnitude;
				if (swipeDistVertical > minSwipeDistY) {
					float swipeValue = Mathf.Sign (touch.position.y - startPos.y);
					if (swipeValue > 0)//up swipe
						Jump ();
				}
				float swipeDistHorizontal = (new Vector3 (touch.position.x, 0, 0) - new Vector3 (startPos.x, 0, 0)).magnitude;
				if (swipeDistHorizontal > minSwipeDistX) {
					float swipeValue = Mathf.Sign (touch.position.x - startPos.x);
					if (swipeValue > 0) {//right swipe 
						//punchright
					} else if (swipeValue < 0) {//left swipe
						//punchleft
					}
				}
				break;
			}
		}
		#endif

	}
	
	void FixedUpdate() {
		//movement on desktop
		#if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_BLACKBERRY && !UNITY_WINRT || UNITY_EDITOR
			Move (Input.GetAxisRaw ("Horizontal"));
			if (Input.GetButtonDown ("Jump"))
				Jump();
		#else
			Move (hInput);
		#endif
	}


	//movement functions
	//jump function
	public void Jump () {
		//on the ground?
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		
		if (grounded) {
			rigidbody2D.AddForce(new Vector2(0, jumpForce));
		}
	}
	//move function
	void Move(float horizonalInput)
	{
		Vector2 moveVel = rigidbody2D.velocity;
		moveVel.x = horizonalInput * maxSpeed;
		rigidbody2D.velocity = moveVel;
	}
	
	public void startMoving(float horizontalInput) {
		hInput = horizontalInput;
	}

	//TODO: implement basic attack functions
}
