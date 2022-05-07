using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenPostProcess : MonoBehaviour
{

    private Volume volume;
    private LensDistortion distort;
    private ColorAdjustments colorAdj;
    //[SerializeField]private float step;

    private void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet<LensDistortion>(out distort);
        volume.profile.TryGet<ColorAdjustments>(out colorAdj);
    }


    public IEnumerator TimeStopEffect()
    {
        for (int i = 0; i < 25; i++)
        {
            distort.intensity.value += 0.04f;
            distort.scale.value -= 0.04f;
            colorAdj.saturation.value -= 4f;
            colorAdj.postExposure.value += -0.02f;
            yield return new WaitForSeconds(0.015f);
        }
        for (int i = 0; i < 25; i++)
        {
            distort.intensity.value -= 0.04f;
            distort.scale.value += 0.04f;
            yield return new WaitForSeconds(0.015f);
        }
        

    }

    public void ResetScreenPostProcess()
    {
        colorAdj.saturation.value = 0f;
        colorAdj.postExposure.value = 0f;
    }

}
