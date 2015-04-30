using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public enum AudioType
    {
        Music,
        Ambience,
        SFX
    }

    public enum SoundType
    {
        GrabUnit,
        DropUnit,
        Promote
    }

    [SerializeField]
    private AudioClip[] m_Songs = null;
    private int m_CurrentSongID = 0;

    [SerializeField]
    private AudioClip[] m_SFX = null;
    
    [SerializeField]
    private AudioSource m_MusicPlayer = null;

    [SerializeField]
    private AudioSource m_AmbiencePlayer = null;

    [SerializeField]
    private AudioSource m_SFXPlayer = null;

    //Singleton
    private static AudioManager m_Instance;
    public static AudioManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType<AudioManager>();
            }

            return m_Instance;
        }
    }

    void Start()
    {
        //Load the volumes from the PlayerPrefs
        LoadVolumes();

        //Start Playing the first song
        if (m_Songs.Length > 0)
        {
            PlaySong(m_CurrentSongID);
        }

        //Start playing the ambience
        m_AmbiencePlayer.loop = true;
        m_AmbiencePlayer.Play();
    }

	// Update is called once per frame
    private void Update()
    {
	    if (!m_MusicPlayer.isPlaying)
        {
            PlayNextSong();
        }
	}

    private void PlayNextSong()
    {
        if (m_Songs.Length > 0)
        {
            ++m_CurrentSongID;
            if (m_CurrentSongID >= m_Songs.Length) m_CurrentSongID = 0;

            PlaySong(m_CurrentSongID);
        }
    }

    private void PlaySong(int id)
    {
        m_MusicPlayer.loop = false;
        m_MusicPlayer.clip = m_Songs[id];
        m_MusicPlayer.Play();
    }

    public void PlaySound(SoundType sound)
    {
        int soundID = (int)sound;
        if (soundID >= m_SFX.Length) return;

        m_SFXPlayer.PlayOneShot(m_SFX[soundID]);
    }

    //Setters
    public void LoadVolumes()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        }

        if (PlayerPrefs.HasKey("AmbienceVolume"))
        {
            SetAmbienceVolume(PlayerPrefs.GetFloat("AmbienceVolume"));
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        }
    }

    public void SetVolume(AudioType audioType, float volume)
    {
        switch (audioType)
        {
            case AudioType.Music:
                SetMusicVolume(volume);
                break;

            case AudioType.Ambience:
                SetAmbienceVolume(volume);
                break;

            case AudioType.SFX:
                SetSFXVolume(volume);
                break;
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (m_MusicPlayer == null)
            return;

        m_MusicPlayer.volume = volume;

        //Save in playerprefs
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetAmbienceVolume(float volume)
    {
        if (m_AmbiencePlayer == null)
            return;

        m_AmbiencePlayer.volume = volume;

        //Save in playerprefs
        PlayerPrefs.SetFloat("AmbienceVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (m_SFXPlayer == null)
            return;

        m_SFXPlayer.volume = volume;

        //Save in playerprefs
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    //Getters
    public float GetVolume(AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.Music:
                return GetMusicVolume();
                //break;

            case AudioType.Ambience:
                return GetAmbienceVolume();
                //break;

            case AudioType.SFX:
                return GetSFXVolume();
                //break;

            default:
                break;
        }

        return 0.0f;
    }

    public float GetMusicVolume()
    {
        if (m_MusicPlayer == null)
            return 0.0f;

        return m_MusicPlayer.volume;
    }

    public float GetAmbienceVolume()
    {
        if (m_AmbiencePlayer == null)
            return 0.0f;

        return m_AmbiencePlayer.volume;
    }

    public float GetSFXVolume()
    {
        if (m_SFXPlayer == null)
            return 0.0f;

        return m_SFXPlayer.volume;
    }
}
