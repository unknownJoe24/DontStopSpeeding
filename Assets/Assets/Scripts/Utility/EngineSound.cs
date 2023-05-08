using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour
{
    [SerializeField] private AudioClip startingSound;
    [SerializeField] private AudioClip engineClip;
    [SerializeField] private float pitchMultiplier = 0.2f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float minVolume = 0.2f;
    [SerializeField] private float maxVolume = 0.5f;
    [Range(0f, 1f)]
    [SerializeField]private float volume = 1f;

    private Rigidbody rb;
    private AudioSource engineAudioSource;
    private bool hasPlayedStartingSound = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        engineAudioSource = SoundManager.Instance.gameObject.AddComponent<AudioSource>();
        engineAudioSource.clip = engineClip;
        engineAudioSource.loop = true;
        engineAudioSource.Play();
    }

    private void Update()
    {
        if(engineAudioSource != null)
        {
            float speed = rb.velocity.magnitude;
            float pitch = Mathf.Clamp(speed * pitchMultiplier, 0, maxPitch);
            float volume = Mathf.Lerp(minVolume, maxVolume, speed / rb.maxAngularVelocity);

            engineAudioSource.pitch = pitch;
            engineAudioSource.volume = volume;

            if (!hasPlayedStartingSound)
            {
                SoundManager.Instance.Play(startingSound, volume);
                hasPlayedStartingSound = true;
            }
        }
    }

    private void OnDisable()
    {
        engineAudioSource.Stop();
        Destroy(engineAudioSource);
    }
}
