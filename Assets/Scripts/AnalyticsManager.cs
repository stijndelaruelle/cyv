using UnityEngine;
using System.Collections;

public class AnalyticsManager : MonoBehaviour
{
    //Taken from: https://developers.google.com/analytics/devguides/collection/unity/v3/reference

    [SerializeField]
    private GoogleAnalyticsV3 m_GoogleAnalytics;

    //Singleton
    private static AnalyticsManager m_Instance;
    public static AnalyticsManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType<AnalyticsManager>();
            }

            return m_Instance;
        }
    }

    //Functions
    public void Start()
    {
        // Start a new session.
        m_GoogleAnalytics.StartSession();
    }

    public void OnDestroy()
    {
        m_GoogleAnalytics.StopSession();
    }

    public void LogScreen(string screenName)
    {
        //Example LogScreen("Main Menu");
        m_GoogleAnalytics.LogScreen(screenName);
    }

    public void LogEvent(string eventCategory, string eventAction, string eventLabel, long value)
    {
        //Example: LogEvent("Achievement", "Unlocked", "Slay 10 dragons", 5);
        m_GoogleAnalytics.LogEvent(eventCategory, eventAction, eventLabel, value);
    }

    public void LogException(string exceptionDescription, bool isFatal)
    {
        //Example: LogException("Incorrect input exception", true);
        m_GoogleAnalytics.LogException(exceptionDescription, isFatal);
    }

    public void LogTiming(string timingCategory, long timingInterval, string timingName, string timingLabel)
    {
        //Example: LogTiming("Loading", 50L, "Main Menu", "First Load");
        m_GoogleAnalytics.LogTiming(timingCategory, timingInterval, timingName, timingLabel);
    }

    public void LogSocial(string socialNetwork, string socialAction, string socialTarget)
    {
        //Example: LogSocial("twitter", "retweet", "twitter.com/googleanalytics/status/482210840234295296");
        m_GoogleAnalytics.LogSocial(socialNetwork, socialAction, socialTarget);
    }

    public void LogTransaction(string transID, string affiliation, double revenue, double tax, double shipping)
    {
        //Example: LogTransaction("TRANS001", "Coin Store", 3.0, 0.0, 0.0);
        m_GoogleAnalytics.LogTransaction(transID, affiliation, revenue, tax, shipping);
    }

    public void LogTransaction(string transID, string affiliation, double revenue, double tax, double shipping, string currencyCode)
    {
        //Example: LogTransaction("TRANS001", "Coin Store", 3.0, 0.0, 0.0, "USD");
        m_GoogleAnalytics.LogTransaction(transID, affiliation, revenue, tax, shipping, currencyCode);
    }

    public void LogItem(string transID, string name, string SKU, string category, double price, long quantity)
    {
        //Example: LogItem("TRANS001", "Sword", "SWORD1223", "Weapon", 3.0, 2);
        m_GoogleAnalytics.LogItem(transID, name, SKU, category, price, quantity);
    }

    public void LogItem(string transID, string name, string SKU, string category, double price, long quantity, string currencyCode)
    {
        //Example: LogItem("TRANS001", "Sword", "SWORD1223", "Weapon", 3.0, 2, "USD");
        m_GoogleAnalytics.LogItem(transID, name, SKU, category, price, quantity, currencyCode);
    }

    //Other stuff: Custom Dimensions & Metrics / Campaigns
}
