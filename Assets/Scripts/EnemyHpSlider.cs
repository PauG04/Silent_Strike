using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpSlider : MonoBehaviour
{
    //[SerializeField] private Enemy enemy;
    [SerializeField] private GameObject slider;
    private SliderBar hpSlider;

    [SerializeField] private float offsetY;
    [SerializeField] private float timeToDesapear;

    private void Awake()
    {
        hpSlider = slider.GetComponent<SliderBar>();

        hpSlider.SetMaxValue(100);
        hpSlider.SetCurrentValue(75);
        //hpSlider.SetMaxValue(enemy.GetMaxHealth());
        //hpSlider.SetCurrentValue(enemy.GetCurrentHealth());

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
        //hpSlider.SetCurrentValue(enemy.GetCurrentHealth());

        Invoke("DisableSlider", timeToDesapear);
    }

    private void DisableSlider()
    {
        slider.SetActive(false);
    }




}
