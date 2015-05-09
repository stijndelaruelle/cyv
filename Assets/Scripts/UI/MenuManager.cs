﻿using UnityEngine;
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
    public void Awake()
    {
        ShowInGameMenu(); //This is just so everything get's initialized!
        ShowMainMenu();
    }

    public void OnEnable()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        if (m_MainMenuPanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_MainMenuPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Main Menu");
    }

    public void ShowNewGame()
    {
        if (m_NewGamePanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_NewGamePanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("New Game");
    }

    public void ShowManual()
    {
        if (m_ManualPanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_ManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Manual");
    }

    public void ShowOptions()
    {
        if (m_OptionsPanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_OptionsPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Options");
    }

    public void ShowCredits()
    {
        if (m_CreditsPanel == null)
            return;

        HideAll();
        ShowCommon(true);
        m_CreditsPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Credits");
    }

    public void ShowInGameMenu()
    {
        if (m_InGamePanel == null)
            return;

        HideAll();
        ShowCommon(false);
        m_InGamePanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Resume");
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

    public bool IsInManual()
    {
        return m_ManualPanel.activeInHierarchy;
    }
}
