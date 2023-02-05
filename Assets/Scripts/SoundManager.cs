using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource audioSource;
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

    public AudioSource PlaySound(AudioClip[] clip, Vector3 position, bool isLooping = false)
    {
        return PlaySound(clip[Random.Range(0, clip.Length)], position, isLooping);
    }

    public AudioSource PlaySound(AudioClip clip, Vector3 position, bool isLooping = false)
    {
        AudioSource audio = Instantiate(audioSource, position, Quaternion.identity);
        audio.name = "SoundObject";
        audio.loop = isLooping;
        audio.clip = clip;
        audio.spatialBlend = 1f;
        audio.dopplerLevel = 0f;
        audio.Play();
        if (!isLooping) Destroy(audio.gameObject, clip.length);

        return audio;
    }

    public AudioSource PlaySound(AudioClip[] clip, Transform attachedTransform, bool isLooping = false)
    {
        return PlaySound(clip[Random.Range(0, clip.Length)], attachedTransform, isLooping);
    }

    public AudioSource PlaySound(AudioClip clip, Transform attachedTransform, bool isLooping = false)
    {
        AudioSource audio = Instantiate(audioSource, attachedTransform);
        audio.name = "SoundObject";
        audio.loop = isLooping;
        audio.clip = clip;
        audio.spatialBlend = 1f;
        audio.dopplerLevel = 0f;
        audio.Play();
        if (!isLooping) Destroy(audio.gameObject, clip.length);

        return audio;
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
