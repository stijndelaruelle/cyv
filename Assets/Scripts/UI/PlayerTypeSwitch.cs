using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerTypeSwitch : MonoBehaviour
{
    [SerializeField]
    private PlayerColor m_PlayerColor;

    [SerializeField]
    private Toggle m_ToggleHuman;

    private void Start()
    {
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        PlayerType playerType = PlayerType.Human;
        if (!m_ToggleHuman.isOn) playerType = PlayerType.AI;

        if (m_PlayerColor == PlayerColor.White)
        {
            GameplayManager.Instance.NewGameSetup.m_WhitePlayerType = playerType;
        }
        else
        {
            GameplayManager.Instance.NewGameSetup.m_BlackPlayerType = playerType;
        }

        GameplayManager.Instance.UpdateNewGameSetup();
    }
}
