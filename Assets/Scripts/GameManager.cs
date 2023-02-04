using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private AudioClip testSound;
    [SerializeField] private AudioClip testLoopSound;

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

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlaySound(testLoopSound, Camera.main.transform, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SoundManager.Instance.PlaySoundOneShot(testSound);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SoundManager.Instance.PlaySound(testSound, Vector3.zero);
        }
    }
}
