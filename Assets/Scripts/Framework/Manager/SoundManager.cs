using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioSource m_AudioMusic;
    public AudioSource m_AudioSound;

    private void Awake()
    {
        m_AudioMusic = this.gameObject.AddComponent<AudioSource>();
        m_AudioMusic.loop = true;
        m_AudioMusic.playOnAwake = false;

        m_AudioSound = this.gameObject.AddComponent<AudioSource>();
        m_AudioSound.loop = false;
    }

    public float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        }
        set
        {
            m_AudioMusic.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    public float SoundVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        }
        set
        {
            m_AudioSound.volume = value;
            PlayerPrefs.SetFloat("SoundVolume", value);
        }
    }


    /*******************       Lua方法调用接口（方法直接包装在里面）        *********************/
    //播放 - 音乐
    public void PlayMusic(string name)
    {
        if (MusicVolume < 0.1)
            return;

        string oldName = "";
        if (m_AudioMusic.clip != null)
            oldName = m_AudioMusic.name;
        if(oldName ==  name)
        {
            m_AudioMusic.Play();
            return;
        }

        Manager.Resource.LoadMusic(name, (UnityEngine.Object obj) =>
        {
            m_AudioMusic.clip = obj as AudioClip;
            m_AudioMusic.Play();
        });
    }

    //播放 - 音效
    public void PlaySound(string name)
    {
        if (SoundVolume < 0.1)
            return;

        Manager.Resource.LoadSound(name, (UnityEngine.Object obj) =>
        {
            m_AudioSound.PlayOneShot(obj as AudioClip);
        });
    }

    //音量调整 - 音乐
    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
    }

    //音量调整 - 音效
    public void SetSoundVolume(float volume)
    {
        SoundVolume = volume;
    }

    //播放暂停
    public void PauseMusic()
    {
        m_AudioMusic.Pause();
    }

    //播放开始
    public void UnPauseMusic()
    {
        m_AudioMusic.UnPause();
    }

    //播放结束
    public void StopMusic()
    {
        m_AudioMusic.Stop();
    }
}
