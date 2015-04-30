using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    private AudioManager.AudioType m_AudioType = AudioManager.AudioType.Music;

    [SerializeField]
    private Slider m_Slider;

    //Functions
	void Start ()
    {
        if (m_Slider == null)
            return;

        m_Slider.value = AudioManager.Instance.GetVolume(m_AudioType);
	}
	
    public void UpdateSlider()
    {
        AudioManager.Instance.SetVolume(m_AudioType, m_Slider.value);
    }
}
