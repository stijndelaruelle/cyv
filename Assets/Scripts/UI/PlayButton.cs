using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    private Button m_Button;

    [SerializeField]
    private Text m_Text;

    private void Start()
    {
        OnNewGameSetupChange();

        GameplayManager.Instance.OnNewGameSetupChange += OnNewGameSetupChange;
    }

    private void OnNewGameSetupChange()
    {
        if (GameplayManager.Instance.NewGameSetup.m_WhitePlayerType == PlayerType.AI && 
            GameplayManager.Instance.NewGameSetup.m_BlackPlayerType == PlayerType.AI)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    private void Activate()
    {
        m_Button.interactable = true;

        Color color = m_Text.color;
        m_Text.color = new Color(color.r, color.g, color.b, 1.0f);
    }

    private void Deactivate()
    {
        m_Button.interactable = false;

        Color color = m_Text.color;
        m_Text.color = new Color(color.r, color.g, color.b, 0.5f);
    }
}
