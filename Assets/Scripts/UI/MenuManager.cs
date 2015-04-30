using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Super quick & dirty
public class MenuManager : MonoBehaviour
{
    //--------------
    // Datamemebers
    //--------------
    [SerializeField]
    private GameObject m_CommonPanel = null;

    [SerializeField]
    private GameObject m_MainMenuPanel = null;

    [SerializeField]
    private GameObject m_NewGamePanel = null;

    [SerializeField]
    private GameObject m_ManualPanel = null;

    [SerializeField]
    private GameObject m_OptionsPanel = null;

    [SerializeField]
    private GameObject m_CreditsPanel = null;

    [SerializeField]
    private GameObject m_InGamePanel = null;

    [SerializeField]
    private GameObject m_EndGamePanel = null;

    //Singleton
    private static MenuManager m_Instance;
    public static MenuManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType<MenuManager>();
            }

            return m_Instance;
        }
    }
    
    //--------------
    // Functions
    //--------------
    public void ShowMainMenu()
    {
        if (m_MainMenuPanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_MainMenuPanel.SetActive(true);
    }

    public void ShowNewGame()
    {
        if (m_NewGamePanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_NewGamePanel.SetActive(true);
    }

    public void ShowManual()
    {
        if (m_ManualPanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_ManualPanel.SetActive(true);
    }

    public void ShowOptions()
    {
        if (m_OptionsPanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_OptionsPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        if (m_CreditsPanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_CreditsPanel.SetActive(true);
    }

    public void ShowInGameMenu()
    {
        if (m_InGamePanel == null)
            return;

        HideAll();
        ShowCommon(false);
        m_InGamePanel.SetActive(true);
    }

    public void ShowEndGameMenu(bool show)
    {
        if (m_EndGamePanel == null)
            return;

        if (!show)
        {
            m_EndGamePanel.SetActive(show);
            return;
        }

        HideAll();
        ShowCommon(false);

        m_InGamePanel.SetActive(true);
        m_EndGamePanel.SetActive(show);
    }

    private void ShowCommon(bool state)
    {
        m_CommonPanel.SetActive(state);
    }

    private void HideAll()
    {
        //Deactivate everything
        if (m_MainMenuPanel != null)
            m_MainMenuPanel.SetActive(false);

        if (m_NewGamePanel != null)
            m_NewGamePanel.SetActive(false);

        if (m_ManualPanel != null)
            m_ManualPanel.SetActive(false);

        if (m_OptionsPanel != null)
            m_OptionsPanel.SetActive(false);

        if (m_CreditsPanel != null)
            m_CreditsPanel.SetActive(false);

        if (m_InGamePanel != null)
            m_InGamePanel.SetActive(false);

        if (m_EndGamePanel != null)
            m_EndGamePanel.SetActive(false);
    }
}
