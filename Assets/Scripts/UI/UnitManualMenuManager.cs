using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Super quick & dirty
public class UnitManualMenuManager : MonoBehaviour
{
    //--------------
    // Datamemebers
    //--------------
    [SerializeField]
    private GameObject m_MainUnitManualPanel = null;

    [SerializeField]
    private GameObject m_KingUnitManualPanel = null;

    [SerializeField]
    private GameObject m_RabbleUnitManualPanel = null;

    [SerializeField]
    private GameObject m_MountainUnitManualPanel = null;

    [SerializeField]
    private GameObject m_LightHorseUnitManualPanel = null;

    [SerializeField]
    private GameObject m_SpearUnitManualPanel = null;

    [SerializeField]
    private GameObject m_CrossbowUnitManualPanel = null;

    [SerializeField]
    private GameObject m_HeavyHorseUnitManualPanel = null;

    [SerializeField]
    private GameObject m_ElephantUnitManualPanel = null;

    [SerializeField]
    private GameObject m_CatapultUnitManualPanel = null;

    [SerializeField]
    private GameObject m_DragonUnitManualPanel = null;

    //--------------
    // Functions
    //--------------
    public void Awake()
    {
        ShowMainUnitManual();
    }

    public void OnEnable()
    {
        ShowMainUnitManual();
    }

    public void ShowMainUnitManual()
    {
        if (m_MainUnitManualPanel == null)
            return;

        HideAll();
        m_MainUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Main Unit Manual");
    }

    public void ShowKingUnitManual()
    {
        if (m_KingUnitManualPanel == null)
            return;

        HideAll();
        m_KingUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("King Unit Manual");
    }

    public void ShowRabbleUnitManual()
    {
        if (m_RabbleUnitManualPanel == null)
            return;

        HideAll();
        m_RabbleUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Rabble Unit Manual");
    }

    public void ShowMountainUnitManual()
    {
        if (m_MountainUnitManualPanel == null)
            return;

        HideAll();
        m_MountainUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Mountain Unit Manual");
    }

    public void ShowLightHorseUnitManual()
    {
        if (m_LightHorseUnitManualPanel == null)
            return;

        HideAll();
        m_LightHorseUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Light Horse Unit Manual");
    }

    public void ShowSpearUnitManual()
    {
        if (m_SpearUnitManualPanel == null)
            return;

        HideAll();
        m_SpearUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Spear Unit Manual");
    }

    public void ShowCrossbowUnitManual()
    {
        if (m_CrossbowUnitManualPanel == null)
            return;

        HideAll();
        m_CrossbowUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Crossbow Unit Manual");
    }

    public void ShowHeavyHorseUnitManual()
    {
        if (m_HeavyHorseUnitManualPanel == null)
            return;

        HideAll();
        m_HeavyHorseUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Heavy Horse Unit Manual");
    }

    public void ShowElephantUnitManual()
    {
        if (m_ElephantUnitManualPanel == null)
            return;

        HideAll();
        m_ElephantUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Elephant Unit Manual");
    }

    public void ShowCatapultUnitManual()
    {
        if (m_CatapultUnitManualPanel == null)
            return;

        HideAll();
        m_CatapultUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Catapult Unit Manual");
    }

    public void ShowDragonUnitManual()
    {
        if (m_DragonUnitManualPanel == null)
            return;

        HideAll();
        m_DragonUnitManualPanel.SetActive(true);

        AnalyticsManager.Instance.LogScreen("Dragon Unit Manual");
    }

    private void HideAll()
    {
        //Deactivate everything
        if (m_MainUnitManualPanel != null)
            m_MainUnitManualPanel.SetActive(false);

        if (m_KingUnitManualPanel != null)
            m_KingUnitManualPanel.SetActive(false);

        if (m_RabbleUnitManualPanel != null)
            m_RabbleUnitManualPanel.SetActive(false);

        if (m_MountainUnitManualPanel != null)
            m_MountainUnitManualPanel.SetActive(false);

        if (m_LightHorseUnitManualPanel != null)
            m_LightHorseUnitManualPanel.SetActive(false);

        if (m_SpearUnitManualPanel != null)
            m_SpearUnitManualPanel.SetActive(false);

        if (m_CrossbowUnitManualPanel != null)
            m_CrossbowUnitManualPanel.SetActive(false);

        if (m_HeavyHorseUnitManualPanel != null)
            m_HeavyHorseUnitManualPanel.SetActive(false);

        if (m_ElephantUnitManualPanel != null)
            m_ElephantUnitManualPanel.SetActive(false);

        if (m_CatapultUnitManualPanel != null)
            m_CatapultUnitManualPanel.SetActive(false);

        if (m_DragonUnitManualPanel != null)
            m_DragonUnitManualPanel.SetActive(false);
    }
}
