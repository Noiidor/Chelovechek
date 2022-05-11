using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSkill : MonoBehaviour
{
    [SerializeField] private PlayerController pCont;
    [SerializeField] private TimeStopAbility timeStopAbility;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private LayerMask expMask;
    [SerializeField] float expRadius;
    [SerializeField] float expForce;
    [SerializeField] float maxDistance;
    [Space]
    [SerializeField] private List<Collider[]> collidersList = new List<Collider[]>();
    [SerializeField] private List<Vector3> hitpointsList;
    [SerializeField] private List<GameObject> vfxList;

    void Update()
    {
        if (Input.GetKeyDown(pCont.fireKey))
        {
            anim.Play("Human Arms Click", 0, 0.0f);
            Shoot();

        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        Physics.Raycast(pCont.camera.ScreenToWorldPoint(Vector3.zero), pCont.camera.transform.forward, out hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore);
        if (hit.point != Vector3.zero)
        {
            if (timeStopAbility.timeStopped)
            {
                var boom = Instantiate(explosionVFX, hit.point + hit.normal / 3, hit.transform.rotation);
                vfxList.Add(boom);
                StartCoroutine(PauseVFX(boom));
                hitpointsList.Add(hit.point);
            }
            else
            {
                var boom = Instantiate(explosionVFX, hit.point + hit.normal / 3, hit.transform.rotation);
                Collider[] colliders = Physics.OverlapSphere(hit.point, expRadius, expMask);
                foreach (Collider nearbyObj in colliders)
                {
                    Rigidbody rb = nearbyObj.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        if (nearbyObj.GetComponent<PlayerController>())
                        {
                            rb.AddExplosionForce(expForce * 1.5f, hit.point, expRadius);
                        }
                        else
                        {
                            rb.AddExplosionForce(expForce, hit.point, expRadius);
                        }
                    }
                }
                Destroy(boom, 1f);
            }
        }
    }


    public void ResumeExplosion()
    {
        for (int i = 0; i < hitpointsList.Count ; i++) 
        {
            Collider[] colliders = Physics.OverlapSphere(hitpointsList[i], expRadius, expMask);
            foreach (Collider nearbyObj in colliders)
            {
                if (nearbyObj.GetComponent<PlayerController>())
                {
                    Rigidbody rb = nearbyObj.GetComponent<Rigidbody>();
                    rb.AddExplosionForce(expForce * 2f, hitpointsList[i], expRadius);
                }
                else
                {
                    Rigidbody rb = nearbyObj.GetComponent<Rigidbody>();
                    rb.AddExplosionForce(expForce, hitpointsList[i], expRadius);
                }
            }
            vfxList[i].GetComponent<ParticleSystem>().Play();
            Destroy(vfxList[i], 0.85f);
        }
        vfxList.Clear();
        collidersList.Clear();
        hitpointsList.Clear();
    }

    IEnumerator PauseVFX(GameObject obj)
    {
        yield return new WaitForSeconds(0.15f);
        obj.GetComponent<ParticleSystem>().Pause();
    }
}
