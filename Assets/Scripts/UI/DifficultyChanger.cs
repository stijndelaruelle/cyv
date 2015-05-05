using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DifficultyChanger : MonoBehaviour
{
    [SerializeField]
    private Toggle m_Toggle;

    [SerializeField]
    private Text m_TextLabel;

    private string[] m_VisualDifficultyType;
    private int m_ID = 1; //Stanger: Standard Medium

    private void Awake()
    {
        m_VisualDifficultyType = new string[9];
        m_VisualDifficultyType[0] = "Drunkard";  //Standard Easy
        m_VisualDifficultyType[1] = "Stanger";   //Standard Medium
        m_VisualDifficultyType[2] = "Wizard";    //Standard Hard

        m_VisualDifficultyType[3] = "Merchant";  //Aggressive Easy
        m_VisualDifficultyType[4] = "Sellsword"; //Aggressive Medium
        m_VisualDifficultyType[5] = "Knight";    //Aggressive Hard

        m_VisualDifficultyType[6] = "Beggar";    //Defensive Easy
        m_VisualDifficultyType[7] = "Smuggler";  //Defensive Medium
        m_VisualDifficultyType[8] = "Banker";    //Defensive Hard
    }

    private void Start()
    {
        //Set tdefault state (Stranger)
        UpdateSettings();
    }

    public void ChangeDifficultyUp()
    {
        m_ID -= 1;
        if (m_ID < 0) m_ID = m_VisualDifficultyType.Length - 1;

        //Enable AI to be certain
        m_Toggle.isOn = true;

        UpdateSettings();
    }

    public void ChangeDifficultyDown()
    {
        m_ID += 1;
        if (m_ID >= m_VisualDifficultyType.Length) m_ID = 0;

        //Enable AI to be certain
        m_Toggle.isOn = true;

        UpdateSettings();
    }

    private void UpdateSettings()
    {
        //Determine the difficulty
        int difficulty = (m_ID % 3);
        GameplayManager.Instance.NewGameSetup.m_AIDifficulty = difficulty;

        //Determine the PlayStyle
        int type = (m_ID / 3);
        GameplayManager.Instance.NewGameSetup.m_AIType = (AIType)type;

        //Set the text label
        m_TextLabel.text = m_VisualDifficultyType[m_ID].ToString();
    }
}
