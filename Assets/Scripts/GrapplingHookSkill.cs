using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookSkill : MonoBehaviour
{
    [SerializeField] private PlayerController pCont;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float maxDistance;
    private LineRenderer line;
    private Vector3 hitPoint;
    private Rigidbody hitRb;
    private bool airFallCanChange;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shoot();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            hitPoint = Vector3.zero;
            hitRb = null;
            line.enabled = false;
            if (airFallCanChange)
            {
                pCont.airFallingEnabled = true;
            }
            
        }
    }


    private void Shoot()
    {
            
        if (hitPoint == Vector3.zero)
        {
            RaycastHit hit;
            Physics.Raycast(pCont.camera.ScreenToWorldPoint(Vector3.zero), pCont.camera.transform.forward, out hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore);
            if (hit.point != Vector3.zero)
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Rigidbody"))
                {
                    hitRb = hit.rigidbody;
                    hitPoint = hit.rigidbody.transform.InverseTransformPoint(hit.point); // Переводит точку попадания из мировых координат в локальные координаты rigidbody
                }
                else
                {
                    hitPoint = hit.point;
                }
            }
        }
        else
        {
            if (hitRb != null)
            {
                hitRb.AddForceAtPosition((pCont.camera.ViewportToWorldPoint(new Vector3(0.8f, 0.25f, 1f)) - hitRb.transform.TransformPoint(hitPoint)) * 50f, hitRb.transform.TransformPoint(hitPoint), ForceMode.Force);
                pCont.playerRb.AddForce((hitRb.transform.TransformPoint(hitPoint) - transform.position) * 50f, ForceMode.Force);
                line.SetPosition(0, pCont.camera.ViewportToWorldPoint(new Vector3(0.8f, 0.25f, 0.5f)));
                line.SetPosition(1, hitRb.transform.TransformPoint(hitPoint));
            }
            else
            {
                if (!pCont.airFallingEnabled)
                {
                    airFallCanChange = false;
                }
                pCont.airFallingEnabled = false;
                pCont.playerRb.AddForce((hitPoint - transform.position) * 25f, ForceMode.Force);
                line.SetPosition(0, pCont.camera.ViewportToWorldPoint(new Vector3(0.8f, 0.25f, 0.5f)));
                line.SetPosition(1, hitPoint);
            }
            line.enabled = true;
        }
    }

}
