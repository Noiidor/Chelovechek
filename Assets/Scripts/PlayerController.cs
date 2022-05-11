using UnityEngine;

public class PlayerController : MonoBehaviour
{

	
	private float jumpBufferCounter;
	private float moveSpeed;
	private bool isSprinting;
	private RaycastHit slopeHit;
	private Vector3 moveVector;
	private Transform stairsCheck;
	private Transform groundCheck;
	private float savedColliderHeight;
	private float savedCameraHeight;
	

	//Переменные, значение которым присваивается только 1 раз
	private float xRotation = 0f;
	private float jumpBufferTime = 0.1f;
	private float dist = 2.5f;

	[HideInInspector] public Rigidbody playerRb;
	[HideInInspector] public bool onRb;
	[HideInInspector] public Rigidbody pulledRb = null;

	[Header("Keys")]
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;
	[SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
	[SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
	[SerializeField] private KeyCode pullKey = KeyCode.E;
	[SerializeField] public KeyCode fireKey = KeyCode.Mouse0;
	[SerializeField] public KeyCode abilityKey = KeyCode.Alpha1;
	[Space]

	[Header("Assigments")]
	[SerializeField] public Camera camera;
	[SerializeField] private CapsuleCollider bodyCollider;
	[SerializeField] private LayerMask groundLayerMask;
	[SerializeField] private LayerMask rigidbodyMask;
	[SerializeField] private TimeStopAbility tStopAbility;
	[Space]

	[Header("Variables")]
	[SerializeField] public bool isGrounded;
	[SerializeField] public bool airFallingEnabled;
	[Range(1, 2)]
	[SerializeField] private float lookSens;
	[SerializeField] private float walkSpeed;
	[SerializeField] private float sprintSpeed;
	[SerializeField] private float jumpFalloff;
	[SerializeField] private float jumpStrenght;
	[Range(0.5f, 0.9f)]
	[SerializeField] private float crouchHeight;


	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		savedColliderHeight = bodyCollider.height;
		savedCameraHeight = camera.transform.localPosition.y;
		stairsCheck = transform.Find("StairsCheck");
		groundCheck = transform.Find("GroundCheck");
		playerRb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		HandleStairs();
		SprintMovement();
		HandleMovement();
		AirFall();
		Jump();
		OnSlope();
		DragControl();
		DebugCheck();
		PullRb();
		OnRigidbody();
		HandleCrouch();
	}

	private void Update()
	{
		HandleCamera();
		JumpBuffer();
		MyInputs();
	}

	private void MyInputs()
	{
		if (Input.GetKeyUp(pullKey))
		{
			ReleaseRb();
		}
	}

	private void DebugCheck()
	{
		//Debug.DrawRay(bodyCollider.bounds.center, playerRb.velocity, Color.green, 0.03f, false);
		//Debug.DrawRay(bodyCollider.bounds.center, moveVector, Color.red, 0.03f, false);
		//Debug.DrawRay(bodyCollider.bounds.center, -slopeHit.normal, Color.blue, 0.03f, false);
		//Debug.Log(playerRb.velocity.magnitude);
	}

	private void HandleMovement()
	{
		float moveX = Input.GetAxisRaw("Horizontal");
		float moveZ = Input.GetAxisRaw("Vertical");

		moveVector = transform.right * moveX + transform.forward * moveZ;
		if (moveVector.magnitude > 1)
			moveVector /= moveVector.magnitude;

		
		Vector3 slopeMoveVector = Vector3.ProjectOnPlane(moveVector, slopeHit.normal); //Проецирует вектор движения параллельно наклонной поверхности
		//if (slopeMoveVector.magnitude != 0 && slopeMoveVector.magnitude < 1)
		//    slopeMoveVector /= slopeMoveVector.magnitude;

		if (playerRb.velocity.magnitude < 0.3f)
		{
			playerRb.velocity = Vector3.zero;
		}

		//Движение на земле и в воздухе
		if (IsGrounded())
		{
			playerRb.AddForce(slopeMoveVector * moveSpeed, ForceMode.Acceleration);
			
			//if (OnSlope() && slopeHit.normal.y < 0.6f)
			//{
			//	rb.AddForce(-transform.up * 10000);
			//}
		}
		else
		{
			playerRb.AddForce(moveVector * moveSpeed / 7f, ForceMode.Acceleration);
		}
	}

	private void HandleCamera()
	{
		float lookX = Input.GetAxis("Mouse X") * lookSens;
		float lookY = Input.GetAxis("Mouse Y") * lookSens;
		

		xRotation -= lookY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		playerRb.transform.Rotate(Vector3.up * lookX);
		camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

		if (isSprinting && moveVector.magnitude != 0)
		{
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 85f, 7f * Time.deltaTime);
		}
		else
		{
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 75f, 7f * Time.deltaTime);
		}
	}

	private void SprintMovement()
	{

		if (Input.GetKey(sprintKey))
		{
			moveSpeed = sprintSpeed;
			isSprinting = true;
		}
		else
		{
			moveSpeed = walkSpeed;
			isSprinting = false;
		}
	}

	private void HandleStairs()
	{
		int maxSteps = 70;
		float stepValue = 0.01f;
		float currentStepValue = 0f;
		RaycastHit hit;

		if (moveVector != Vector3.zero)
		{
			for (int stepsCount = 0; stepsCount < maxSteps; stepsCount++)
			{
				Physics.Raycast(stairsCheck.position + new Vector3(0f, currentStepValue, 0f), moveVector, out hit, 0.8f, groundLayerMask, QueryTriggerInteraction.Ignore);
				if (hit.normal == Vector3.zero)
				{
					playerRb.position += transform.up * currentStepValue;
					break;
				}
				currentStepValue += stepValue;
			}
		}
	}

	private void HandleCrouch()
	{
		if (Input.GetKey(crouchKey))
		{
			bodyCollider.height = Mathf.Lerp(bodyCollider.height, savedColliderHeight * crouchHeight, 0.5f);
			bodyCollider.center = new Vector3(0f, Mathf.Lerp(bodyCollider.center.y, -(savedColliderHeight - bodyCollider.height) / 2, 0.5f) , 0f);
			camera.transform.localPosition = new Vector3(0f, Mathf.Lerp(camera.transform.localPosition.y, savedCameraHeight - (savedColliderHeight - bodyCollider.height), 0.5f) , 0f);
		}
		else
		{
			bodyCollider.height = Mathf.Lerp(bodyCollider.height, savedColliderHeight, 0.5f);
			bodyCollider.center = Vector3.Lerp(bodyCollider.center, Vector3.zero, 0.5f);
			camera.transform.localPosition = new Vector3(0f, Mathf.Lerp(camera.transform.localPosition.y, savedCameraHeight, 0.5f), 0f);
		}
	}

	private void StairsMovement()
	{
		RaycastHit groundHit;
		Vector3 rayHitPoint;
		Vector3 targetPos;

		Physics.Raycast((bodyCollider.bounds.center - new Vector3(0f, bodyCollider.bounds.extents.y - 0.5f, 0f)), -transform.up, out groundHit, 1.5f, groundLayerMask, QueryTriggerInteraction.Ignore);
		
		if (groundHit.normal == Vector3.zero)
		{
			rayHitPoint = playerRb.position - transform.up;
		}
		else
		{
			rayHitPoint = groundHit.point;
		}

		targetPos = playerRb.position;
		if (IsGrounded())
		{
			targetPos = rayHitPoint + transform.up;
		}
		else
		{
			targetPos = rayHitPoint;
		}


		if (IsGrounded())
		{
			//playerRb.position =  Vector3.Lerp(playerRb.position, targetPos, 0.6f);
			playerRb.position = targetPos;
		}
	}

	private void Jump()
	{
		//Coyote time
		if (jumpBufferCounter > 0f && IsGrounded())
		{
			playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
			playerRb.AddForce(transform.up * jumpStrenght, ForceMode.Impulse);
		}
	}

	private void JumpBuffer()
	{
		if (Input.GetKeyDown(jumpKey))
		{
			jumpBufferCounter = jumpBufferTime;
		}
		else
		{
			jumpBufferCounter -= Time.deltaTime;
		}
	}

	private void DragControl()
	{
		if (IsGrounded())
		{
			playerRb.drag = 10f;
			
		}
		else
		{
			playerRb.drag = 1f;
		}
	}

	private void AirFall()
	{
		//Прижимает игрока после прохождения апогея прыжка
		if (airFallingEnabled)
		{
			if (!IsGrounded() && playerRb.velocity.y < jumpFalloff && playerRb.useGravity)
				playerRb.AddForce(-transform.up * jumpStrenght / 17, ForceMode.Impulse);
		}
	}
	
	private bool IsGrounded()
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, 0.40f, groundLayerMask, QueryTriggerInteraction.Ignore);
		return isGrounded;
	}

	private bool OnSlope()
	{
		Physics.Raycast(bodyCollider.bounds.center, -transform.up, out slopeHit, bodyCollider.bounds.extents.y + 2f);
		if (slopeHit.normal != Vector3.up && slopeHit.normal != Vector3.zero)
		{
			return true;
		}
		else
		{
			return false;
		}

	}

	private void PullRb()
	{
		if (Input.GetKey(pullKey))
		{
			// Если игрок стоит на предмете который берет, то отпускает его
			if (onRb)
			{
				ReleaseRb();
			}
			else
			{
				RaycastHit dragHit;
				if (pulledRb != null)
				{
					if (tStopAbility.timeStopped)
					{
						pulledRb.constraints = RigidbodyConstraints.None;
					}
					pulledRb.AddForce(((camera.ScreenToWorldPoint(Vector3.zero) + camera.transform.forward * dist) - pulledRb.position) * 10, ForceMode.VelocityChange); //Держит предмет на заданном от игрока расстоянии
				}
				else
				{
					if (Physics.Raycast(camera.ScreenToWorldPoint(Vector3.zero), camera.transform.forward, out dragHit, 2.5f, rigidbodyMask))
					{
						dist = Vector3.Distance(camera.ScreenToWorldPoint(Vector3.zero), dragHit.rigidbody.position); //Запоминает расстояние от игрока до предмета
						pulledRb = dragHit.rigidbody;
						pulledRb.angularDrag = 20f;
						pulledRb.drag = 20f;
						pulledRb.useGravity = false;
					}
				}
			}
			
		}
	}

	private void ReleaseRb()
	{
		if (pulledRb != null)
		{
			if (tStopAbility.timeStopped)
			{
				pulledRb.constraints = RigidbodyConstraints.FreezeAll;
			}
			pulledRb.useGravity = true;
			pulledRb.drag = 0f;
			pulledRb.angularDrag = 0.05f;
			pulledRb.velocity /= 2;
			pulledRb = null;
		}
			
	}

	private void OnRigidbody()
	{
		RaycastHit hit;
		Physics.Raycast(groundCheck.position, -transform.up, out hit, 1f, rigidbodyMask, QueryTriggerInteraction.Ignore);
		onRb = pulledRb == hit.rigidbody && pulledRb != null ? true : false; // true если игрок пытается держать предмет на котором стоит
		if (hit.rigidbody != null)
		{
			if (playerRb.velocity.magnitude < hit.rigidbody.velocity.magnitude) // Добавляет нехватающую игроку скорость до предмета снизу
			{
				playerRb.velocity = moveVector + hit.rigidbody.velocity*1.4f;
				if (Vector3.Angle(hit.rigidbody.velocity, moveVector) > 160f || Vector3.Angle(hit.rigidbody.velocity, moveVector) < 20f)
				{
					playerRb.AddForce(moveVector*5f, ForceMode.VelocityChange);
				}
			}
		}
	}
}
