using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopAbility : MonoBehaviour
{

    [SerializeField] private PlayerController pCont;
    [SerializeField] private ExplosionSkill expSkill;
    [SerializeField] private Animator anim;
    [SerializeField] private ScreenPostProcess screenDistort;
    [SerializeField] private SoundManager sManager;
    [SerializeField] private LayerMask rbMask;
    [SerializeField] private float abilityRadius;

    [HideInInspector] public List<Rigidbody> rbList;
    [HideInInspector] public List<Vector3> rbVelocityList;
    [HideInInspector] public List<Vector3> rbAngularVelocityList;

    [HideInInspector] public bool timeStopped;


    void Update()
    {
        if (Input.GetKeyDown(pCont.abilityKey))
        {
            //anim.Play("Human Arms Cast", 0, 0.0f);
            TimeStop();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && timeStopped)
        {
            StartCoroutine(CoroutineTimeResume());
        }
    }

    private void TimeStop()
    {
        if (!timeStopped)
        {
            timeStopped = true;
            sManager.Play("TimeStop");
            StartCoroutine(screenDistort.TimeStopEffect());
            Collider[] colliders = Physics.OverlapSphere(transform.position, abilityRadius, rbMask);
            foreach (Collider nearbyObj in colliders)
            {
                Rigidbody rb = nearbyObj.GetComponent<Rigidbody>();
                rbVelocityList.Add(rb.velocity);
                rbAngularVelocityList.Add(rb.angularVelocity);
                rbList.Add(rb);
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    private void TimeResume()
    {
        if (timeStopped)
        {
            foreach (Rigidbody rb in rbList)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.velocity = rbVelocityList[0];
                rbVelocityList.RemoveAt(0);
                rb.angularVelocity = rbAngularVelocityList[0];
                rbAngularVelocityList.RemoveAt(0);
            }
            screenDistort.ResetScreenPostProcess();
            expSkill.ResumeExplosion();
            timeStopped = false;
            rbList.Clear();
        }
    }

    private IEnumerator CoroutineTimeResume()
    {
        sManager.Play("TimeResume");
        yield return new WaitForSeconds(1.5f);
        TimeResume();
    }
}
