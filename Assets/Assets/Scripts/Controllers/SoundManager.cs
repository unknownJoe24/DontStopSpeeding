using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Audio players components.
    [SerializeField]
    private AudioSource EffectsSource;

    [SerializeField]
    private AudioSource MusicSource;

    [SerializeField]
    private AudioSource AVSource;

    // Random pitch adjustment range.
    [SerializeField]
    private float LowPitchRange = .95f;
    [SerializeField]
    private float HighPitchRange = 1.05f;

    // Singleton instance.
    public static SoundManager Instance { get; private set; }

    // Initialize the singleton instance.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // Play a single clip through the sound effects source.
    public void Play(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            EffectsSource.clip = clip;
            EffectsSource.volume = volume;
            EffectsSource.Play();
        }
    }

    // Play the audio clip that accompanies the video.
    public void PlayAV(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            AVSource.clip = clip;
            AVSource.volume = volume;
            AVSource.Play();
        }
    }

    // Play a single clip through the music source with a specified volume.
    public void PlayMusic(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            MusicSource.clip = clip;
            MusicSource.volume = volume;
            MusicSource.Play();
        }
    }

    // Stop playing the current music.
    public void StopMusic()
    {
        MusicSource.Stop();
    }

    // Play a random clip from an array, and randomize the pitch slightly.
    public void RandomSoundEffect(AudioClip[] clips, float volume)
    {
/*
        if (_source == null)
            _source = EffectsSource;
*/
        if (clips.Length == 0) return;

        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectsSource.pitch = randomPitch;
        EffectsSource.volume = volume;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }
}