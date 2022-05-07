using UnityEngine;

public class PlayerController : MonoBehaviour
{


	private Rigidbody playerRb;
	private float xRotation = 0f;
	private float jumpBufferTime = 0.1f;
	private float jumpBufferCounter;
	private float moveSpeed;
	private bool isSprinting;
	private RaycastHit slopeHit;
	private Vector3 slopeMoveVector;
	private Vector3 moveVector;
	private Vector3 rbMoveVector;
	public bool onRb = false;
	private float dist = 2.5f;
	[HideInInspector] public Rigidbody pulledRb = null;

	[Header("Keys")]
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;
	[SerializeField] private KeyCode sprintKey = KeyCode.LeftControl;
	[SerializeField] private KeyCode pullKey = KeyCode.E;
	[SerializeField] public KeyCode fireKey = KeyCode.Mouse0;
	[SerializeField] public KeyCode abilityKey = KeyCode.Alpha1;
	[Space]

	[Header("Assigments")]
	[SerializeField] public Camera camera;
	[SerializeField] private Transform checkSphere;
	[SerializeField] private Collider bodyCollider;
	[SerializeField] private LayerMask groundLayerMask;
	[SerializeField] private LayerMask dragLayerMask;
	[SerializeField] private LayerMask rigidbodyMask;
	[SerializeField] private TimeStopAbility tStopAbility;
	[Space]

	[Header("Variables")]
	[SerializeField] private bool isGrounded;
	[SerializeField] private float lookSens;
	[SerializeField] private float walkSpeed;
	[SerializeField] private float sprintSpeed;
	[SerializeField] private float jumpFalloff;
	[SerializeField] private float jumpStrenght;



	void Start()
	{
		playerRb = GetComponent<Rigidbody>();
		Cursor.lockState = CursorLockMode.Locked;
	}

	void FixedUpdate()
	{
		SprintMovement();
		HandleMovement();
		AirFall();
		Jump();
		OnSlope();
		DragControl();
		DebugCheck();
		StairsMovement();
		PullRb();
		OnRigidbody();
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
		Debug.DrawRay(bodyCollider.bounds.center, playerRb.velocity, Color.green, 0.03f, false);
		Debug.DrawRay(bodyCollider.bounds.center, moveVector, Color.red, 0.03f, false);
		Debug.DrawRay(bodyCollider.bounds.center, -slopeHit.normal, Color.blue, 0.03f, false);
		//Debug.Log(rb.velocity.magnitude);
	}

	private void HandleMovement()
	{
		float moveX = Input.GetAxisRaw("Horizontal");
		float moveZ = Input.GetAxisRaw("Vertical");

		moveVector = transform.right * moveX + transform.forward * moveZ;
		//if (moveVector.magnitude > 1)
		//	moveVector /= moveVector.magnitude;

		slopeMoveVector = Vector3.ProjectOnPlane(moveVector, slopeHit.normal);
		//if (slopeMoveVector.magnitude != 0 && slopeMoveVector.magnitude < 1)
		//	slopeMoveVector /= slopeMoveVector.magnitude;


		if (IsGrounded())
		{
			if (moveVector == Vector3.zero)
            {
				playerRb.useGravity = false;
            }
            else
            {
				playerRb.useGravity = true;
            }
			playerRb.AddForce(slopeMoveVector * moveSpeed, ForceMode.Acceleration);
			//if (OnSlope() && slopeHit.normal.y < 0.6f)
			//{
			//	rb.AddForce(Vector3.down * 10000);
			//}
		}
		else
		{
			playerRb.useGravity = true;
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

	private void StairsMovement()
	{
		RaycastHit groundHit;
		Vector3 rayHitPoint;
		Vector3 targetPos;

		Physics.Raycast((bodyCollider.bounds.center - new Vector3(0f, bodyCollider.bounds.extents.y - 0.5f, 0f)), Vector3.down, out groundHit, 1.2f, groundLayerMask, QueryTriggerInteraction.Ignore);

		if (groundHit.normal == Vector3.zero)
		{
			rayHitPoint = playerRb.position - new Vector3(0f, 1f, 0f);
		}
		else
		{
			rayHitPoint = groundHit.point;
		}

		targetPos = playerRb.position;
		if (IsGrounded())
		{
			targetPos.y = rayHitPoint.y + 1f;
		}
		else
		{
			targetPos.y = rayHitPoint.y;
		}


		if (IsGrounded())
		{

			playerRb.position = Vector3.Lerp(playerRb.position, targetPos, 0.6f);
		}
	}

	private void Jump()
	{
		if (jumpBufferCounter > 0f && IsGrounded())
		{
			playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
			playerRb.AddForce(Vector3.up * jumpStrenght, ForceMode.Impulse);
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
		if (playerRb.velocity.y < jumpFalloff && IsGrounded() == false)
			playerRb.AddForce(Vector3.down * jumpStrenght / 17, ForceMode.Impulse);
	}

	private bool IsGrounded()
	{
		isGrounded = Physics.CheckSphere(checkSphere.position, 0.40f, groundLayerMask, QueryTriggerInteraction.Ignore);
		return isGrounded;
	}

	private bool OnSlope()
	{
		Physics.Raycast(bodyCollider.bounds.center, Vector3.down, out slopeHit, bodyCollider.bounds.extents.y + 2f);
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
					pulledRb.AddForce(((camera.ScreenToWorldPoint(Vector3.zero) + camera.transform.forward * dist) - pulledRb.position) * 10, ForceMode.VelocityChange);
				}
				else
				{
					try
					{
						Physics.Raycast(camera.ScreenToWorldPoint(Vector3.zero), camera.transform.forward, out dragHit, 2.5f, dragLayerMask);
						dist = Vector3.Distance(camera.ScreenToWorldPoint(Vector3.zero), dragHit.rigidbody.position);
						pulledRb = dragHit.rigidbody;
						pulledRb.angularDrag = 20f;
						pulledRb.drag = 20f;
						pulledRb.useGravity = false;
					}
					catch (System.NullReferenceException)
					{ }
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
		Physics.Raycast(checkSphere.position, Vector3.down, out hit, 1f, rigidbodyMask, QueryTriggerInteraction.Ignore);
		onRb = pulledRb == hit.rigidbody && pulledRb != null ? true : false; 
		Debug.Log(pulledRb);
		Debug.Log(hit.rigidbody);
		Debug.Log(onRb);
		//if (pulledRb == hit.rigidbody)
  //      {
		//	onRb = true;
  //      }
  //      else
  //      {
		//	onRb = false;
		//}
		if (hit.rigidbody != null)
        {
			if (playerRb.velocity.magnitude < hit.rigidbody.velocity.magnitude)
            {

				//if (Vector3.Angle(hit.rigidbody.velocity, slopeMoveVector) > 165 || Vector3.Angle(hit.rigidbody.velocity, slopeMoveVector) < 15)
				//            {
				//	Debug.Log(Vector3.Angle(hit.rigidbody.velocity, slopeMoveVector));
				//	slopeMoveVector *= 3f;
				//            }

				//playerRb.AddForce((hit.rigidbody.velocity - playerRb.velocity)*120f, ForceMode.Acceleration);
				playerRb.velocity = slopeMoveVector + hit.rigidbody.velocity*1.4f;
				if (Vector3.Angle(hit.rigidbody.velocity, moveVector) > 160f || Vector3.Angle(hit.rigidbody.velocity, moveVector) < 20f)
				{
					//Debug.Log(Vector3.Angle(hit.rigidbody.velocity, slopeMoveVector));
					playerRb.AddForce(moveVector*5f, ForceMode.VelocityChange);
                }
				//playerRb.AddForce((hit.rigidbody.velocity) * 0.5f, ForceMode.VelocityChange);
			}
        }
        
        
    }

}
