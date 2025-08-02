using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource soundObject;

    public AudioSource musicSource;

    public bool muteMusic = false;

    public float musicVolume;

    private void Awake()
    {
        // This is the code to ensure there's only one Sound Manager in a scene at a time
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // This is the code that carries the SM over between scenes
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (musicSource)
        {
            musicVolume = musicSource.volume;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            muteMusic = !muteMusic;
        }
        if (musicSource)
        {
            if (muteMusic)
            {
                musicSource.volume = 0f;
            }
            else
            {
                musicSource.volume = musicVolume;
            }
        }
    }

    public void PlaySoundClip(AudioClip audioClip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (audioClip == null)
        {
            return;
        }
        // spawn in gameObject to play the sound
        AudioSource audioSource = Instantiate(soundObject, position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.Play();
        float clipLength = audioClip.length / audioSource.pitch;
        DontDestroyOnLoad(audioSource.gameObject);
        Destroy(audioSource.gameObject, clipLength);
    }
}
