using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TakeHitEffect : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float animDuration = 3;
    [SerializeField] private float maxIntensity = 0.3f;

    private Vignette vignette;
    private float animTime = 0;

    void Start()
    {
        MainTree.Instance.OnTakeDamage += OnTreeTakeDamage;
        volume.profile.TryGet<Vignette>(out vignette);
    }

    private void OnDestroy()
    {
        MainTree.Instance.OnTakeDamage -= OnTreeTakeDamage;
    }

    void Update()
    {
        if (animTime < animDuration)
        {
            animTime += Time.deltaTime;
            animTime = Mathf.Max(0, animTime);
            vignette.intensity.value = animationCurve.Evaluate(animTime / animDuration);
        }
        else if (vignette.intensity.value > 0)
        {
            vignette.intensity.value = 0;
        }
    }

    private void OnTreeTakeDamage(float health, float damage)
    {
        TakeHit();
    }

    public void TakeHit()
    {
        animTime = 0;
    }


}
