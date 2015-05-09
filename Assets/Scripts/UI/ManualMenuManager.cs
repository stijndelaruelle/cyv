using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Super quick & dirty
public class ManualMenuManager : MonoBehaviour
{
    //--------------
    // Datamemebers
    //--------------
    [SerializeField]
    private GameObject m_MainManualPanel = null;

    [SerializeField]
    private GameObject m_GameplayManualPanel = null;

    [SerializeField]
    private GameObject m_UnitsManualPanel = null;

    //--------------
    // Functions
    //--------------
    public void Awake()
    {
        ShowMainManual();
    }

    public void OnEnable()
    {
        ShowMainManual();
    }

    public void ShowMainManual()
    {
        if (m_MainManualPanel == null)
            return;

        HideAll();
        m_MainManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Main Manual");
    }

    public void ShowGameplayManual()
    {
        if (m_GameplayManualPanel == null)
            return;

        HideAll();
        m_GameplayManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Gameplay Manual");
    }

    public void ShowUnitsManual()
    {
        if (m_UnitsManualPanel == null)
            return;

        HideAll();
        m_UnitsManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Units Manual");
    }

    private void HideAll()
    {
        //Deactivate everything
        if (m_MainManualPanel != null)
            m_MainManualPanel.SetActive(false);

        if (m_GameplayManualPanel != null)
            m_GameplayManualPanel.SetActive(false);

        if (m_UnitsManualPanel != null)
            m_UnitsManualPanel.SetActive(false);
    }
}
