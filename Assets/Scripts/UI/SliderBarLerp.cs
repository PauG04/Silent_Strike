using UnityEngine;
using UnityEngine.UI;

public class SliderBarLerp : MonoBehaviour
{
    [SerializeField] private Image lerpMask;    
    [SerializeField] private Image mask;        
    [SerializeField] private float lerpDuration;

    private float lerpProgress = 0;

    private void Update()
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
        //lerpProgress = Mathf.Clamp01(lerpProgress); 

        lerpMask.fillAmount = Mathf.Lerp(lerpMask.fillAmount, mask.fillAmount, lerpProgress);
    }
}