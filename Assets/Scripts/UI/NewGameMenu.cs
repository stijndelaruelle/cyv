using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    [SerializeField]
    private Toggle m_ToggleHumanWhite;

    [SerializeField]
    private Toggle m_ToggleHumanBlack;

    [SerializeField]
    private Toggle m_TogglePassAndPlay;

    //Functions
	public void StartGame()
    {
        //Determine the player types
        PlayerType playerTypeWhite = PlayerType.Human;
        if (!m_ToggleHumanWhite.isOn) playerTypeWhite = PlayerType.AI;

        PlayerType playerTypeBlack = PlayerType.Human;
        if (!m_ToggleHumanBlack.isOn) playerTypeBlack = PlayerType.AI;

        //Determine the gamemode
        GameMode gameMode = GameMode.PassAndPlay;
        if (!m_TogglePassAndPlay.isOn) gameMode = GameMode.TabletPlay;

        GameplayManager.Instance.NewGame(playerTypeWhite, playerTypeBlack, gameMode);
        MenuManager.Instance.ShowInGameMenu();
	}
}
