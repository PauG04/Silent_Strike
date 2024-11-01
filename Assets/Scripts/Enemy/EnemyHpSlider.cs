using UnityEngine;

public class EnemyHpSlider : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] private GameObject slider;
    private SliderBar hpSlider;

    [SerializeField] private float offsetY;
    [SerializeField] private float timeToDesapear;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        hpSlider = slider.GetComponent<SliderBar>();

        hpSlider.SetMaxValue(enemy.GetMaxHp());
        hpSlider.SetCurrentValue(enemy.GetCurrentHp());

        slider.SetActive(false);
    }

    private void Update()
    {
        slider.transform.position = transform.position + new Vector3(0, offsetY, 0);
    }

    public void UpdateSlider()
    {
        slider.SetActive(true);

        EnableSlider();
    }

    private void EnableSlider()
    {
        slider.SetActive(true);
        hpSlider.SetCurrentValue(enemy.GetCurrentHp());

        Invoke("DisableSlider", timeToDesapear);
    }

    private void DisableSlider()
    {
        slider.SetActive(false);
    }
}
