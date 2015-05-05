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
        #if UNITY_ANDROID
            // Start a new session.
            m_GoogleAnalytics.StartSession();
        #endif
    }

    public void OnDestroy()
    {
        #if UNITY_ANDROID
            m_GoogleAnalytics.StopSession();
        #endif
    }

    public void LogScreen(string screenName)
    {
        #if UNITY_ANDROID
            //Example LogScreen("Main Menu");
            m_GoogleAnalytics.LogScreen(screenName);
        #endif
    }

    public void LogEvent(string eventCategory, string eventAction, string eventLabel, long value)
    {
        #if UNITY_ANDROID
            //Example: LogEvent("Achievement", "Unlocked", "Slay 10 dragons", 5);
            m_GoogleAnalytics.LogEvent(eventCategory, eventAction, eventLabel, value);
        #endif
    }

    public void LogException(string exceptionDescription, bool isFatal)
    {
        #if UNITY_ANDROID
            //Example: LogException("Incorrect input exception", true);
            m_GoogleAnalytics.LogException(exceptionDescription, isFatal);
        #endif
    }

    public void LogTiming(string timingCategory, long timingInterval, string timingName, string timingLabel)
    {
        #if UNITY_ANDROID
            //Example: LogTiming("Loading", 50L, "Main Menu", "First Load");
            m_GoogleAnalytics.LogTiming(timingCategory, timingInterval, timingName, timingLabel);
        #endif
    }

    public void LogSocial(string socialNetwork, string socialAction, string socialTarget)
    {
        #if UNITY_ANDROID
            //Example: LogSocial("twitter", "retweet", "twitter.com/googleanalytics/status/482210840234295296");
            m_GoogleAnalytics.LogSocial(socialNetwork, socialAction, socialTarget);
        #endif
    }

    public void LogTransaction(string transID, string affiliation, double revenue, double tax, double shipping)
    {
        #if UNITY_ANDROID
            //Example: LogTransaction("TRANS001", "Coin Store", 3.0, 0.0, 0.0);
            m_GoogleAnalytics.LogTransaction(transID, affiliation, revenue, tax, shipping);
        #endif
    }

    public void LogTransaction(string transID, string affiliation, double revenue, double tax, double shipping, string currencyCode)
    {
        #if UNITY_ANDROID
            //Example: LogTransaction("TRANS001", "Coin Store", 3.0, 0.0, 0.0, "USD");
            m_GoogleAnalytics.LogTransaction(transID, affiliation, revenue, tax, shipping, currencyCode);
        #endif
    }

    public void LogItem(string transID, string name, string SKU, string category, double price, long quantity)
    {
        #if UNITY_ANDROID
            //Example: LogItem("TRANS001", "Sword", "SWORD1223", "Weapon", 3.0, 2);
            m_GoogleAnalytics.LogItem(transID, name, SKU, category, price, quantity);
        #endif
    }

    public void LogItem(string transID, string name, string SKU, string category, double price, long quantity, string currencyCode)
    {
        #if UNITY_ANDROID
            //Example: LogItem("TRANS001", "Sword", "SWORD1223", "Weapon", 3.0, 2, "USD");
            m_GoogleAnalytics.LogItem(transID, name, SKU, category, price, quantity, currencyCode);
        #endif
    }

    //Other stuff: Custom Dimensions & Metrics / Campaigns
}
