using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewGameSwitch : MonoBehaviour
{
    [SerializeField]
    private Button m_Button;

    [SerializeField]
    private Text m_Text;

	private void Start()
    {
        m_Button.interactable = false;

        Color color = m_Text.color;
        m_Text.color = new Color(color.r, color.g, color.b, 0.5f);

        GameplayManager.Instance.OnNewGame += OnNewGame;
	}
	
    private void OnNewGame()
    {
        m_Button.interactable = true;

        Color color = m_Text.color;
        m_Text.color = new Color(color.r, color.g, color.b, 1.0f);
    }
}
