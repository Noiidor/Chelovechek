using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGravity : MonoBehaviour
{

    private HashSet<Rigidbody> affectedRigidbodies = new HashSet<Rigidbody>();
    private SphereCollider triggerCollider;
    private Rigidbody planetRb;

    void Start()
    {
        triggerCollider = GetComponent<SphereCollider>();
        planetRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Rigidbody affectedRigidbody in affectedRigidbodies)
        {
            Vector3 gravDirection = (transform.position - affectedRigidbody.position).normalized;
            float gravStrenght = (planetRb.mass * affectedRigidbody.mass) / (Vector3.Distance(transform.position, affectedRigidbody.position));
            affectedRigidbody.AddForce(gravDirection*gravStrenght, ForceMode.Acceleration);
            if (affectedRigidbody.GetComponent<PlayerController>() != null)
            {
                Debug.Log(gravStrenght);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            affectedRigidbodies.Add(other.attachedRigidbody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            affectedRigidbodies.Remove(other.attachedRigidbody);
        }
    }

}
