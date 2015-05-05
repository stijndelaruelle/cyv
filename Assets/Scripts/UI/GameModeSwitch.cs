using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameModeSwitch : MonoBehaviour
{
    [SerializeField]
    private Toggle m_TogglePassAndPlay;

    private void Start()
    {
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        GameMode gameMode = GameMode.PassAndPlay;
        if (!m_TogglePassAndPlay.isOn) gameMode = GameMode.MirroredPlay;

        GameplayManager.Instance.SetGameMode(gameMode);
    }
}
