using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGravityFacing : MonoBehaviour
{
    public List<PlanetGravity> planetGrav;
    [SerializeField] private List<float> planetGravStrenght;


    void FixedUpdate()
    {
        if (planetGrav != null)
        {
            Quaternion targetRot = Quaternion.FromToRotation(-transform.up, planetGrav[CalculateGravDirection()].transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot * transform.rotation, 0.03f);
        }
        
    }

    private int CalculateGravDirection()
    {
        planetGravStrenght.Clear();
        foreach (PlanetGravity planGrav in planetGrav)
        {
            planetGravStrenght.Add(planGrav.playerGravStrenght);
        }
        float maxStrenght = planetGravStrenght.Max();
        return planetGravStrenght.IndexOf(maxStrenght);
    }
}
