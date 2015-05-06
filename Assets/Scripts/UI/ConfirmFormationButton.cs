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
    }

    private void OnChangeGameState(GameState currentGameState, GameState previousGameState)
    {
        if (previousGameState != m_GameState)
            return;

        m_Button.gameObject.SetActive(false);
    }

    private void Update()
    {
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
    }
}
