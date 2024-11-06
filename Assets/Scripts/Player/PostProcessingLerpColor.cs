using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PostProcessingLerpColor : MonoBehaviour
{
    [SerializeField] private float mininValue;
    [SerializeField] private float lerpDuration;
    [SerializeField] private Volume _volume;

    private Vignette _vignette;
    private bool startInterpolate;
    private float lerpProgress;

    void Start()
    {
        if (_volume != null && _volume.profile != null && _volume.profile.TryGet(out _vignette))
        {
            _vignette.active = true; 
        }
    }

    private void Update()
    {
        if(startInterpolate)
        {
            _vignette.color.value = Color.Lerp(Color.black, Color.red, lerpProgress / lerpDuration); ;
            lerpProgress += Time.deltaTime;
        }    
    }

    public void ChangeVignetteColor(float _value)
    {
        if(_value < mininValue) 
        {
            startInterpolate = true;
        }

    }
}
