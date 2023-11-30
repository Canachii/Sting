// 볼륨 조절 슬라이더 값 범위 = 0.0001 ~ 1 (로그10 * 20 = -80 ~ 0)

using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

/// <summary>
/// 오디오 재생 장치
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public Sound[] bgmClips, sfxClips;
    public AudioSource bgmSource, sfxSource;
    public AudioMixer audioMixer;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(ES3.Load("BGM", 1f)) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(ES3.Load("SFX", 1f)) * 20);
        PlayMusic("Main Theme");
    }

    public void PlayMusic(string value)
    {
        var find = Array.Find(bgmClips, sound => sound.name == value);
        
        if (find == null)
        {
            Debug.Log($"Can't find '{value}'");
        }
        else
        {
            bgmSource.clip = find.clip;
            bgmSource.Play();
        }
    }
    
    public void PlaySFX(string value)
    {
        var find = Array.Find(sfxClips, sound => sound.name == value);
        
        if (find == null)
        {
            Debug.Log($"Can't find '{value}'");
        }
        else
        {
            sfxSource.PlayOneShot(find.clip);
        }
    }

    public void SetMusicVolume(float value)
    {
        ES3.Save("BGM", value);
        audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        ES3.Save("SFX", value);
        audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
    }

    public void FadeMusicVolume(bool value)
    {
        if (value)
            audioMixer.DOSetFloat("BGM", -80f, UIController.Duration);
        else
            audioMixer.SetFloat("BGM", Mathf.Log10(ES3.Load("BGM", 1f)) * 20);
    }
}