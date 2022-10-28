using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusiqueManager : SingletonND<MusiqueManager>
{
    public List<AudioClip> audioClips;
    AudioClip ClipPlay;
    AudioSource audioSource;
    private void Start()
    {
        if (audioClips == null) return;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!audioSource.isPlaying && !_PlayClip)
        {
            PlayClip();
        }
    }

    bool _PlayClip = false;
    void PlayClip()
    {
        _PlayClip = true;
        while (audioSource.clip == ClipPlay)
        {
            int mr = Random.Range(0, audioClips.Count);
            audioSource.clip = audioClips[mr];
        }
        ClipPlay = audioSource.clip;
        if (audioSource.isPlaying) return;
        audioSource.Play();
        _PlayClip = false;
    }
}
