using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderBar : MonoBehaviour
{
    [SerializeField] private float maxValue;
    [SerializeField] private float minValue = 0;
    [SerializeField] private float currentValue;
    [SerializeField] private Image mask;
    [SerializeField] private Color color;

    private void Update()
    {
        GetCurrentFill();
    }

    public void GetCurrentFill()
    {
        float currentOffset = currentValue - minValue;
        float maxOffset = maxValue - minValue;
        float fillAmount = currentOffset / maxOffset;
        mask.fillAmount = fillAmount;

        mask.color = color;


    }

    public void SetMaxValue(float value)
    {
        maxValue = value;
    }
    public void SetCurrentValue(float value)
    {
        currentValue = Mathf.Clamp(value, minValue, maxValue);
    }

    public float GetCurrentValue()
    {
        return currentValue;
    }

    public void IncrementCurrentValue(float increment)
    {
        SetCurrentValue(currentValue + increment);
    }
}
