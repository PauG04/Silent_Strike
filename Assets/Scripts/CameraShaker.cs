using System.Collections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    public void Shake(float duration, float positionOffsetStrength)
    {
        StopAllCoroutines();
        StartCoroutine(CameraShakeCoroutine(duration, positionOffsetStrength));
    }

    private IEnumerator CameraShakeCoroutine(float duration, float positionOffsetStrength)
    {
        float elapsed = 0f;
        float currentMagnitude = 1f;

        while (elapsed < duration)
        {
            float x = (Random.value - 0.5f) * currentMagnitude * positionOffsetStrength;
            float y = (Random.value - 0.5f) * currentMagnitude * positionOffsetStrength;

            transform.localPosition = new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            currentMagnitude = (1 - (elapsed / duration)) * (1 - (elapsed / duration));

            yield return null;
        }

        transform.localPosition = Vector3.zero;
    }
}
