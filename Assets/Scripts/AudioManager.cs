using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
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

    [SerializeField]
    private bool m_MuteMusic = false;

    [SerializeField]
    private bool m_MuteSFX = false;

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
        MuteMusic(m_MuteMusic);
        MuteSFX(m_MuteSFX);

        if (m_MuteMusic) return;

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
	    if (!m_MusicPlayer.isPlaying && !m_MuteMusic)
        {
            PlayNextSong();
        }
	}

    private void PlayNextSong()
    {
        if (m_MuteMusic) return;

        if (m_Songs.Length > 0)
        {
            ++m_CurrentSongID;
            if (m_CurrentSongID >= m_Songs.Length) m_CurrentSongID = 0;

            PlaySong(m_CurrentSongID);
        }
    }

    private void PlaySong(int id)
    {
        if (m_MuteMusic) return;

        m_MusicPlayer.loop = false;
        m_MusicPlayer.clip = m_Songs[id];
        m_MusicPlayer.Play();
    }

    public void PlaySound(SoundType sound)
    {
        if (m_MuteSFX) return;

        int soundID = (int)sound;
        if (soundID >= m_SFX.Length) return;

        m_SFXPlayer.PlayOneShot(m_SFX[soundID]);
    }

    public void ToggleMusic()
    {
        MuteMusic(!m_MuteMusic);
    }

    public void ToggleSFX()
    {
        MuteSFX(!m_MuteSFX);
    }

    public void MuteMusic(bool state)
    {
        m_MuteMusic = state;

        if (state)
        {
            m_MusicPlayer.Stop();
            m_AmbiencePlayer.Stop();
        }
        else
        {
            m_MusicPlayer.Play();
            m_AmbiencePlayer.Play();
        }

        m_MusicPlayer.mute = state;
        m_AmbiencePlayer.mute = state;
    }

    public void MuteSFX(bool state)
    {
        m_MuteSFX = state;

        if (state)
        {
            m_SFXPlayer.Stop();
        }
        else
        {
            m_SFXPlayer.Play();
        }

        m_SFXPlayer.mute = state;
    }
}
