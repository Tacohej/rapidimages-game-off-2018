using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource MusicSource;
    private AudioSource ClipSource;

    // Start is called before the first frame update
    void Start()
    {
        GameObject musicSourceObject = new GameObject("MusicSource");
        musicSourceObject.transform.parent = transform;
        GameObject clipSourceObject = new GameObject("ClipSource");
        clipSourceObject.transform.parent = transform;
        
        MusicSource = musicSourceObject.AddComponent<AudioSource>();
        ClipSource = clipSourceObject.AddComponent<AudioSource>();
        //PlayMusicFile("Sound/Unsafe_Waters_01", true);
    }

    public void PlayMusicFile(string musicPath)// bool repeating = false) Cant have multiple arguments if you want to use from a UI button :(
    {
        MusicSource.Stop();
        MusicSource.clip = Resources.Load<AudioClip>(musicPath);
        //MusicSource.loop = repeating;
        MusicSource.loop = true;

        MusicSource.Play();
    }

    public void StartMusic() 
    {
        MusicSource.Play();
    }

    public void PauseMusic()
    {
        MusicSource.Pause();
    }

    public void PlayClipFile(string clipPath)
    {
        ClipSource.PlayOneShot(Resources.Load<AudioClip>(clipPath));
    }
}
