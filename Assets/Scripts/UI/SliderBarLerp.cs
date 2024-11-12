using UnityEngine;
using UnityEngine.UI;

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