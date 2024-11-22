using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PostProcessingLerpColor : MonoBehaviour
{
    [SerializeField] private float mininValue;
    [SerializeField] private float lerpDuration;
    [SerializeField] private float aberrationIntensity;
    [SerializeField] private Volume _volume;

    private Vignette _vignette;
    private FilmGrain _filmGrain;
    private ChromaticAberration _aberration;
    private bool startInterpolate;
    private float lerpProgress;

    private float initIntensity;

    void Start()
    {
        if (_volume != null && _volume.profile != null)
        {
            // Vignette
            if (_volume.profile.TryGet(out _vignette))
            {
                _vignette.active = true;
            }

            // Film Grain
            if (_volume.profile.TryGet(out _filmGrain))
            {
                _filmGrain.active = true;
            }

            // Chromatic Aberration
            if (_volume.profile.TryGet(out _aberration))
            {
                _aberration.active = true;
            }

            initIntensity = _aberration.intensity.value;
        }
    }

    private void Update()
    {
        if(startInterpolate)
        {
            _vignette.color.value = Color.Lerp(Color.black, Color.red, lerpProgress / lerpDuration);
            lerpProgress += Time.deltaTime;
        }    
    }

    public void ChangeVignetteColor(float _value)
    {
        if(_value < mininValue) 
        {
            startInterpolate = true;
            _aberration.intensity.value = aberrationIntensity;
            _filmGrain.type.value = FilmGrainLookup.Large02;
        }
    }

    public void ResetPostPorcessing()
    {
        if (startInterpolate)
        {
            startInterpolate = false;
            _aberration.intensity.value = initIntensity;
            _filmGrain.type.value = FilmGrainLookup.Thin2;
            _vignette.color.value = Color.black;
        }

    }
}
