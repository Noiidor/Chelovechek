using UnityEngine;

public class DimLight : MonoBehaviour
{

    [SerializeField]private float dimTime;
    [SerializeField]private Light light;

    void FixedUpdate()
    {
        light.intensity = Mathf.Lerp(light.intensity, 0, dimTime);
    }
}
