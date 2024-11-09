using TMPro;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    [SerializeField] private float timeToDisappear;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-0.04f, 0.04f), 0f);
        Invoke("DisableText", timeToDisappear);
    }

    private void DisableText()
    {
        this.gameObject.SetActive(false);
    }

    public void SetText(float damage)
    {
        textMeshPro.text = damage.ToString();
    }
}
