using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private void Start()
    {
        SoundManager.Instance.PlayMusic(musicClip, volume);
    }
}