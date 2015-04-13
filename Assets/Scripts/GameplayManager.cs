using UnityEngine;
using System.Collections;

public enum GameState
{
    SetupWhite,
    SetupBlack,
    WhiteTurn,
    BlackTurn,
    EndGame
}

public class GameplayManager : MonoBehaviour
{
    //Singleton
    private static GameplayManager m_Instance;
    public static GameplayManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType<GameplayManager>();
            }

            return m_Instance;
        }
    }

}
