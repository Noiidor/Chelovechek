using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySkill : MonoBehaviour
{

    [SerializeField] private PlayerController pCont;


    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            pCont.playerRb.AddForce(pCont.camera.transform.forward * 200f, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            pCont.playerRb.AddForce(-pCont.camera.transform.forward * 200f, ForceMode.Acceleration);
        }
    }
}
