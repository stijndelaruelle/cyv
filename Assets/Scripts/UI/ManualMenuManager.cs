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
    private GameObject m_BasicManualPanel = null;

    [SerializeField]
    private GameObject m_UnitsManualPanel = null;

    [SerializeField]
    private GameObject m_PromotionManualPanel = null;

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

    public void ShowBasicManual()
    {
        if (m_BasicManualPanel == null)
            return;

        HideAll();
        m_BasicManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Basic Manual");
    }

    public void ShowUnitsManual()
    {
        if (m_UnitsManualPanel == null)
            return;

        HideAll();
        m_UnitsManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Units Manual");
    }

    public void ShowPromotionManual()
    {
        if (m_PromotionManualPanel == null)
            return;

        HideAll();
        m_PromotionManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Promotion Manual");
    }

    private void HideAll()
    {
        //Deactivate everything
        if (m_MainManualPanel != null)
            m_MainManualPanel.SetActive(false);

        if (m_BasicManualPanel != null)
            m_BasicManualPanel.SetActive(false);

        if (m_UnitsManualPanel != null)
            m_UnitsManualPanel.SetActive(false);

        if (m_PromotionManualPanel != null)
            m_PromotionManualPanel.SetActive(false);
    }
}
