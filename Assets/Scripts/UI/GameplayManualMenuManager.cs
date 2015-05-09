using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Super quick & dirty
public class GameplayManualMenuManager : MonoBehaviour
{
    //--------------
    // Datamemebers
    //--------------
    [SerializeField]
    private GameObject m_MainGameplayManualPanel = null;

    [SerializeField]
    private GameObject m_SetupManualPanel = null;

    [SerializeField]
    private GameObject m_VictoryManualPanel = null;

    [SerializeField]
    private GameObject m_PromotionManualPanel = null;

    //--------------
    // Functions
    //--------------
    public void Awake()
    {
        ShowMainGameplayManual();
    }

    public void OnEnable()
    {
        ShowMainGameplayManual();
    }

    public void ShowMainGameplayManual()
    {
        if (m_MainGameplayManualPanel == null)
            return;

        HideAll();
        m_MainGameplayManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Main Gameplay Manual");
    }

    public void ShowSetupGameplayManual()
    {
        if (m_SetupManualPanel == null)
            return;

        HideAll();
        m_SetupManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Setup Gameplay Manual");
    }

    public void ShowVictoryGameplayManual()
    {
        if (m_VictoryManualPanel == null)
            return;

        HideAll();
        m_VictoryManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Victory Gameplay Manual");
    }

    public void ShowPromotionGameplayManual()
    {
        if (m_PromotionManualPanel == null)
            return;

        HideAll();
        m_PromotionManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Promotion Gameplay Manual");
    }

    private void HideAll()
    {
        //Deactivate everything
        if (m_MainGameplayManualPanel != null)
            m_MainGameplayManualPanel.SetActive(false);

        if (m_SetupManualPanel != null)
            m_SetupManualPanel.SetActive(false);

        if (m_VictoryManualPanel != null)
            m_VictoryManualPanel.SetActive(false);

        if (m_PromotionManualPanel != null)
            m_PromotionManualPanel.SetActive(false);
    }
}
