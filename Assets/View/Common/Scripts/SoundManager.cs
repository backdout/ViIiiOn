using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 자동으로 오디오 소스 추가 
[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
  
    public AudioSource BgmSource;
    public List<AudioSource> EffectSources = new List<AudioSource>();
    private bool IsEffectOn;
    private bool IsBgmOn;
    public bool IsInit { get; private set; }
    public enum EffSoundKind
    {
        Hit, Sword, GetGem, Cash, Gold, ButtonTouch,
    }
    public void Init(bool IsChangeScene = false)
    {
        SetData();

        if (IsChangeScene)
            SetBgmSound(IsChangeScene);

        IsInit = true;
    }
    private void SetData()
    {
        IsBgmOn = OptionDataManager.Instance.BgmSound;
        IsEffectOn = OptionDataManager.Instance.EffectSound;
        if (BgmSource == null)
            SetBgmSound(true);
     
        if (EffectSources.Count == 0)
        {
            for (int i = 0; i < System.Enum.GetValues(typeof(EffSoundKind)).Length; i++)
            {
                AudioSource audioSourceClone = gameObject.AddComponent<AudioSource>();
                audioSourceClone.playOnAwake = false;
                audioSourceClone.loop = false;
                EffectSources.Add(audioSourceClone);
            }
        }
    }

    private AudioSource GetAudioSource()
    {
        for (int i = 0; i < EffectSources.Count; i++)
        {
            if (EffectSources[i].isPlaying == false)
                return EffectSources[i];
        }

        AudioSource audioSourceClone = gameObject.AddComponent<AudioSource>();
        audioSourceClone.playOnAwake = false;
        audioSourceClone.loop = false;
        EffectSources.Add(audioSourceClone);

        return audioSourceClone;
    }
    
    public void PlayEffectAudioClip(EffSoundKind soundKind)
    {
        if (!IsEffectOn)
            return;

        PlaySound(GetAudioSource(), GetSound(soundKind.ToString()));
    }

    public void PlaySound(AudioSource source, AudioClip clip, float volume = 1)
    {
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }

    private void SetAudioClipPlay(AudioSource source)
    {

        if (IsBgmOn)
        {
            if (!source.isPlaying)
                source.Play();
        }
        else
        {
            if (source.isPlaying)
                source.Stop();
        }
       
    }

    public void SetBgmSound(bool isChangeScene = false)
    {
        if (BgmSource == null)
        {
            BgmSource = gameObject.GetComponent<AudioSource>();
            BgmSource.loop = true;
            BgmSource.volume = 0.3f;
        }

        if (isChangeScene)
        {
            if (LoadingScenePrefab.Instance.IsUIScene)
                BgmSource.clip = GetSound("MainBgm");
            else
                BgmSource.clip = GetSound("BattleBgm");
        }

        SetAudioClipPlay(BgmSource);
    }

   
    private AudioClip GetSound(string clipName, string path = "")
    {
        AudioClip clip;
        clip = Resources.Load<AudioClip>(path + clipName);
        return clip;
    }

    public void SetIsBgmOn(bool isOn)
    {
        IsBgmOn = isOn;
        SetAudioClipPlay(BgmSource);
    }
    public void SetIsEffOn(bool isOn)
    {
        IsEffectOn = isOn;
        SetAudioClipPlay(BgmSource);
    }
}
