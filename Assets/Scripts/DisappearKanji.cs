using UnityEngine;

public class DisappearKanji : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float timeToDesappear;
    private float timer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timeToDesappear = PlayerController.instance.GetTimeToDesappearKunai();
    }

    private void OnEnable()
    {
        spriteRenderer.color = new Color(255, 255, 255, 1f);
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime / timeToDesappear;

        float currentAlpha = Mathf.Lerp(1f, 0f, timer);

        spriteRenderer.color = new Color(255, 255, 255, currentAlpha);

        if (currentAlpha <= 0f)
            Destroy(this.gameObject);

    }
}
