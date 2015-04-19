using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    private PlayerType m_CurrentPlayer = PlayerType.White;
    public PlayerType CurrentPlayer
    {
        get { return m_CurrentPlayer; }
    }

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

        m_VisualBoard.EnableUnitSelection(m_CurrentPlayer);
    }

    public void Move()
    {
        //A move has been chosen, but not submitted yet!
        EnableGlobalUnitSelection(false);
    }

    public void UndoMove()
    {
        //Super lame, just reload the board!
        m_VisualBoard.LoadBoardState(m_VisualBoard.CurrentBoardState);
        EnableGlobalUnitSelection(true);
    }

    public void SubmitMove()
    {
        m_VisualBoard.SaveBoardState();
        SwapTurns();

        EnableGlobalUnitSelection(true);
    }

    public void SwapTurns()
    {
        if (m_CurrentPlayer == PlayerType.White) { m_CurrentPlayer = PlayerType.Black; }
        else                                     { m_CurrentPlayer = PlayerType.White; }

        m_VisualBoard.EnableUnitSelection(m_CurrentPlayer);
    }

    public void AIMove()
    {
        //Get the current boardstate
        if (m_VisualBoard == null)
            return;

        BoardState currentBoardState = m_VisualBoard.CurrentBoardState;
        currentBoardState.SetCurrentPlayer(m_CurrentPlayer);

        //Calculate all the moves
        DateTime time = DateTime.Now;
        currentBoardState.ProcessAllMoves(m_BoardStates);
        DateTime time2 = DateTime.Now;

        double ms = (time2 - time).TotalMilliseconds;
        Debug.Log("Processing all the moves for " + m_AIMoveDepth + " moves took " + ms + "ms");

        //Now we found our best, let's do that
        currentBoardState.ProcessBestMove();

        //Load visually & done!
        m_VisualBoard.LoadBoardState(currentBoardState);
        SwapTurns();
    }

    private void EnableGlobalUnitSelection(bool state)
    {
        m_VisualBoard.GetComponent<CanvasGroup>().blocksRaycasts = state;
    }
}
