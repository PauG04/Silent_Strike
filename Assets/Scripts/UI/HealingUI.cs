using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealingUI : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Image image;

    private void Update()
    {
        textMeshPro.text = player.GetCurrentHealings().ToString();
        if (player.GetCurrentHealings() <= 0)
            image.color = new Color(255, 255, 255, 0.4f);
        else
            image.color = new Color(255, 255, 255, 1);
    }
}
