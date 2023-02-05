using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private AudioClip soundtrack;

    void Start()
    {
        SoundManager.Instance.PlaySound(soundtrack, Camera.main.transform, true, 0.01f);
    }
}
