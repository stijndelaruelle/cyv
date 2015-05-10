using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConfirmFormationButton : MonoBehaviour
{
    [SerializeField]
    private GameState m_GameState;

    [SerializeField]
    private Button m_Button;

    [SerializeField]
    private Text m_Text;

    private void Start()
    {
        GameplayManager.Instance.OnChangeGameState += OnChangeGameState;
        GameplayManager.Instance.OnNewGame += OnNewGame;
    }

    private void OnNewGame()
    {
        m_Button.gameObject.SetActive(true);
    }

    private void OnChangeGameState(GameState currentGameState, GameState previousGameState)
    {
        if (currentGameState == m_GameState)
            return;

        #if !UNITY_EDITOR
            m_Button.gameObject.SetActive(false);
        #endif
    }

    private void Update()
    {
        #if !UNITY_EDITOR
            if (GameplayManager.Instance.PlacedAllUnits())
            {
                m_Button.interactable = true;

                Color color = m_Text.color;
                m_Text.color = new Color(color.r, color.g, color.b, 1.0f);
            }
            else
            {
                m_Button.interactable = false;

                Color color = m_Text.color;
                m_Text.color = new Color(color.r, color.g, color.b, 0.5f);
            }
        #endif
    }
}
