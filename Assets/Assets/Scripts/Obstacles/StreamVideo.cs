using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StreamVideo : MonoBehaviour
{
    public GameObject RawImage;
    private RawImage rawImage;
    public VideoPlayer videoPlayer;
    //public AudioSource audioSource; // Tutorial artefact

    void Start()
    {
        rawImage = RawImage.GetComponent<RawImage>();
        RawImage.SetActive(false);
        videoPlayer.loopPointReached += CheckOver;
    }

    public IEnumerator PlayVideo()
    {
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }

        RawImage.SetActive(true);
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
        //SoundManager.Instance.PlayAV(INSERT_CLIP_HERE.mp3, 1);
        //audioSource.Play(); // Tutorial artefact
    }

    void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        RawImage.SetActive(false);
        vp.Stop();
    }
}
