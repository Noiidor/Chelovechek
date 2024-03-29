﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class PlanetGravity : MonoBehaviour
{

    private HashSet<Rigidbody> affectedRigidbodies = new HashSet<Rigidbody>();
    private Rigidbody planetRb;
    [HideInInspector] public float playerGravStrenght;
    private PlayerGravityFacing playerGravFacing;

    void Start()
    {
        playerGravFacing = FindObjectOfType<PlayerGravityFacing>();
        planetRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        foreach (Rigidbody affectedRigidbody in affectedRigidbodies)
        {
            Vector3 gravDirection = (transform.position - affectedRigidbody.position).normalized;
            float gravStrenght = planetRb.mass / Vector3.Distance(transform.position, affectedRigidbody.position);
            //float gravStrenght = (planetRb.mass * affectedRigidbody.mass) / Vector3.Distance(transform.position, affectedRigidbody.position);

            if (affectedRigidbody.useGravity)
            {
                if (affectedRigidbody.GetComponent<PlayerController>())
                {
                    playerGravStrenght = gravStrenght;
                }
                affectedRigidbody.AddForce(gravDirection * gravStrenght, ForceMode.Acceleration);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            affectedRigidbodies.Add(other.attachedRigidbody);
        }
        if (other.GetComponent<PlayerController>())
        {
            playerGravFacing.planetGrav.Add(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            affectedRigidbodies.Remove(other.attachedRigidbody);
        }
        if (other.GetComponent<PlayerController>())
        {
            playerGravFacing.planetGrav.Remove(this);
        }
    }

    
    private void Awake()
    {
        Rigidbody planetRb = GetComponent<Rigidbody>();
        planetRb.isKinematic = true;
        SphereCollider gravTrigger = GetComponent<SphereCollider>();
        gravTrigger.isTrigger = true;
        
    }

}
