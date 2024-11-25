using UnityEngine;
using UnityEngine.UI;

public class HealingUI : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [SerializeField] private Sprite barFilled;
    [SerializeField] private Sprite barEmpty;

    [SerializeField] private Image bar1;
    [SerializeField] private Image bar2;
    [SerializeField] private Image bar3;

    private void Awake()
    {
        bar1.sprite = barFilled;
        bar2.sprite = barFilled;
        bar3.sprite = barFilled;
    }

    private void Update()
    {
        if (player.GetCurrentHealings() == 2)
            bar3.sprite = barEmpty;
        else if (player.GetCurrentHealings() == 1)
            bar2.sprite = barEmpty;
        else if (player.GetCurrentHealings() == 0)
            bar1.sprite = barEmpty;

    }
}
