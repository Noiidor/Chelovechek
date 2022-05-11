using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Rigidbody rb;
    private bool reverse = false;
    private Vector3 vector = Vector3.zero;
    private BoxCollider triggerCollider;
    private Vector3 lastPos;
    [SerializeField] private float smoothTime;
    [SerializeField] private float maxSpeed;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private TimeStopAbility timeStopAbility;

    //можно попробовать что бы сама платформа двигала rigidbody, а не парентила их
    //че за хуйня...
    //видимо код можно к хуям удалить

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        //CreateTrigger();
    }

    void FixedUpdate()
    {
        PlatformMovement();
    }

    private void PlatformMovement()
    {
        if (!timeStopAbility.timeStopped)
        {
            if (reverse)
            {
                //transform.position = Vector3.SmoothDamp(rb.position, startPoint.position, ref vector, smoothTime, maxSpeed);
                //rb.velocity = new Vector3.s (rb.position, startPoint.position, ref vector, smoothTime, maxSpeed);
                rb.MovePosition(Vector3.SmoothDamp(rb.position, startPoint.position, ref vector, smoothTime, maxSpeed));
                if (Vector3.Distance(rb.position, startPoint.position) < 1f)
                {
                    reverse = false;
                }
            }
            else
            {
                //transform.position = Vector3.SmoothDamp(rb.position, endPoint.position, ref vector, smoothTime, maxSpeed);
                rb.MovePosition(Vector3.SmoothDamp(rb.position, endPoint.position, ref vector, smoothTime, maxSpeed));
                if (Vector3.Distance(rb.position, endPoint.position) < 1f)
                {
                    reverse = true;
                }
            }
        }
        
    }


    //private void OnTriggerStay(Collider collider)
    //{
    //    lastPos = rb.position;
    //}

    //private void OnTriggerEnter(Collider collider)
    //{
    //    collider.gameObject.transform.parent = transform;
    //}

    //private void OnTriggerExit(Collider collider)
    //{
    //    collider.gameObject.transform.parent = null;
    //    Vector3 trackVelocity = (transform.position - lastPos) / Time.deltaTime;
    //    Rigidbody colRb = collider.gameObject.GetComponent<Rigidbody>();
    //    if (colRb.velocity.magnitude > 0.3f)
    //    {
    //        colRb.AddForce(trackVelocity, ForceMode.VelocityChange);
    //    }
    //    Debug.Log(trackVelocity.magnitude);
    //}


    //private void CreateTrigger()
    //{
    //    triggerCollider = gameObject.AddComponent<BoxCollider>();
    //    triggerCollider.isTrigger = true;
    //    triggerCollider.center += new Vector3(0f, triggerCollider.center.y * 2, 0f);
    //    triggerCollider.size = new Vector3(triggerCollider.size.x - 0.3f, 1f, triggerCollider.size.z - 0.3f);
    //}

}