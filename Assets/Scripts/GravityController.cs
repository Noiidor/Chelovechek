using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{

    private PlayerController pCont;
    private bool gravityAffected;
    private Rigidbody playerRb;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GravityControl();
    }


    private void GravityControl()
    {
        if (pCont.isStanding || !pCont.isGrounded)
        {
            gravityAffected = false;
        }
        else
        {
            gravityAffected = true;
        }
    }


}
