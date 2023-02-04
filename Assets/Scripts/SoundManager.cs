using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource audioSourceOneShot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void PlaySound(AudioClip clip, Vector3 position, bool isLooping = false)
    {
        GameObject soundObject = new GameObject("SoundObject");
        soundObject.transform.position = position;
        AudioSource audio = soundObject.AddComponent<AudioSource>();
        audio.loop = isLooping;
        audio.clip = clip;
        audio.spatialBlend = 1f;
        audio.dopplerLevel = 0f;
        audio.Play();
        if (!isLooping) Destroy(soundObject, clip.length);
    }

    public void PlaySound(AudioClip clip, Transform attachedTransform, bool isLooping = false)
    {
        GameObject soundObject = new GameObject("SoundObject");
        soundObject.transform.parent = attachedTransform;
        soundObject.transform.position = attachedTransform.position;
        AudioSource audio = soundObject.AddComponent<AudioSource>();
        audio.loop = isLooping;
        audio.clip = clip;
        audio.spatialBlend = 1f;
        audio.dopplerLevel = 0f;
        audio.Play();
        if (!isLooping) Destroy(soundObject, clip.length);
    }

    public void PlaySoundOneShot(AudioClip clip)
    {
        if (audioSourceOneShot == null)
        {
            audioSourceOneShot = Camera.main.GetComponentInChildren<AudioSource>();
        }

        audioSourceOneShot.PlayOneShot(clip);
    }
}
