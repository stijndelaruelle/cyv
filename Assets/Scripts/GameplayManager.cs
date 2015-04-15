using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    //------------------
    // Datamembers
    //------------------
    [SerializeField]
    private VisualBoard m_VisualBoard = null;

    [SerializeField]
    private int m_AIMoveDepth = 3;

    private List<BoardState> m_BoardStates = null;

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

    private void Awake()
    {
        m_BoardStates = new List<BoardState>();

        //Generate all our needed boardstates
        for (int i = 0; i < m_AIMoveDepth; ++i)
        {
            BoardState boardState = new BoardState();
            boardState.GenerateDefaultState(6);

            m_BoardStates.Add(boardState);
        }
    }

    public void AIMove()
    {
        //Get the current boardstate
        if (m_VisualBoard == null)
            return;

        BoardState currentBoardState = m_VisualBoard.CurrentBoardState;
        currentBoardState.ProcessAllMoves(m_BoardStates);
    }
}
