using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SliderBarLerp : MonoBehaviour
{
    [SerializeField] private Image lerpMask;    
    [SerializeField] private Image mask;        
    [SerializeField] private float lerpDuration;

    private float lerpProgress = 0;

    private void FixedUpdate()
    {
        if (mask.fillAmount < lerpMask.fillAmount)
        {
            LerpFillAmount();
        }
        else if(mask.fillAmount > lerpMask.fillAmount)
        {
            lerpMask.fillAmount = mask.fillAmount;
        }
        else
        {
            lerpProgress = 0;
        }
    }

    private void LerpFillAmount()
    {
        lerpProgress += Time.deltaTime / lerpDuration;

        float currentProgress = Mathf.Lerp(lerpMask.fillAmount, mask.fillAmount, lerpProgress);

        lerpMask.fillAmount = currentProgress;
    }
}