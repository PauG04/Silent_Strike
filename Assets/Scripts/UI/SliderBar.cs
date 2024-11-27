using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderBar : MonoBehaviour
{
    [SerializeField] private float maxValue;
    [SerializeField] private float currentValue;
    [SerializeField] private Image mask;
    [SerializeField] private Color color;

    private void Update()
    {
        GetCurrentFill();
    }

    public void GetCurrentFill()
    {
        float fillAmount = currentValue / maxValue;
        mask.fillAmount = fillAmount;

        mask.color = color;
    }

    public void SetMaxValue(float value)
    {
        maxValue = value;
    }
    public void SetCurrentValue(float value)
    {
        currentValue = value;
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
