using Unity.VisualScripting;
using UnityEngine;

public class CutKanji : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private GameObject downSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idleSprite;
    }

    public void Cut()
    {
        if (spriteRenderer.sprite != idleSprite)
            return;

        spriteRenderer.sprite = upSprite;

        GenerateCutedKanji();
    }

    private void GenerateCutedKanji()
    {
        Instantiate(downSprite, transform);
    }
}
