using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public enum GameState
{
    Setup,
    Game,
    EndGame
}

public enum PlayerType
{
    Human,
    AI
}

public enum GameMode
{
    PassAndPlay, //Board flips turns after every move (unless AI is involved)
    TabletPlay   //Black units are flipped at the beginning of the game
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

    [SerializeField]
    private GameObject m_ConfirmFormationButton = null; //Only used to confirm a formation, we use the refernce to enable/disable that button

    [SerializeField]
    private Text m_EndGameText = null; //The text displayed when the game ends

    //Stores the playertypes (human or AI)
    private PlayerType[] m_PlayerTypes = new PlayerType[2];

    private int m_CurrentBoardStateID = 0;
    private List<BoardState> m_BoardStates = null; //All the previous boardstates (used for undo/redo/match history)
    private List<BoardState> m_AIBoardStates = null; //The boardstates the AI uses to calculate everything

    private PlayerColor m_CurrentPlayer = PlayerColor.White;
    public PlayerColor CurrentPlayer
    {
        get { return m_CurrentPlayer; }
    }

    private GameState m_GameState = GameState.Setup;
    public GameState GameState
    {
        get { return m_GameState; }
    }

    private GameMode m_GameMode = GameMode.PassAndPlay;

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
        m_AIBoardStates = new List<BoardState>();

        //Generate all our needed boardstates
        for (int i = 0; i < m_AIMoveDepth; ++i)
        {
            BoardState boardState = new BoardState();
            boardState.GenerateDefaultState(BoardState.BOARD_SIZE);

            m_AIBoardStates.Add(boardState);
        }
    }

    public void Start()
    {
        //Not is Awake as they should not depend on eachother there yet
        NewGame(PlayerType.Human, PlayerType.Human, GameMode.TabletPlay);
    }

    public void NewGame(PlayerType playerType1, PlayerType playerType2, GameMode gameMode)
    {
        //Set the playertypes
        m_PlayerTypes[0] = playerType1;
        m_PlayerTypes[1] = playerType2;
        m_CurrentPlayer = PlayerColor.White;

        //Set game state to setup
        SetGameState(GameState.Setup);

        //Clear all the boardstates
        m_CurrentBoardStateID = 0;
        m_BoardStates.Clear();

        //Create an empty board state
        BoardState boardState = new BoardState();
        boardState.GenerateDefaultState(BoardState.BOARD_SIZE);

        m_VisualBoard.SetBoardState(boardState);

        //Flip the board only if the player is black & we're facing a white AI
        if (m_PlayerTypes[0] == PlayerType.AI && m_PlayerTypes[1] == PlayerType.Human)
        {
            m_VisualBoard.FlipBoard(true);
        }

        //Otherwise simply don't
        else
        {
            m_VisualBoard.FlipBoard(false);
        }

        //Set the gamemode (pass & play or tablet)
        SetGameMode(gameMode);

        SaveBoardState();
    }

    public void HighlightSetupZone(bool enable)
    {
        m_VisualBoard.HighlightSetupZone(m_CurrentPlayer, enable);
    }

    public void SaveBoardState()
    {
        //Save the current board
        m_VisualBoard.SaveBoardState();

        //Clear all the extra's
        if (m_CurrentBoardStateID < m_BoardStates.Count - 1)
        {
            int count = m_BoardStates.Count - m_CurrentBoardStateID - 1;
            m_BoardStates.RemoveRange(m_CurrentBoardStateID + 1, count);
        }

        //Create a new boardstate
        BoardState boardState = new BoardState();
        boardState.GenerateDefaultState(BoardState.BOARD_SIZE);

        //Copy the unit data from the current board in there
        boardState.CopyBoard(m_VisualBoard.CurrentBoardState);
 
        m_BoardStates.Add(boardState);

        m_CurrentBoardStateID = m_BoardStates.Count - 1;
    }

    public void LoadBoardState()
    {
        m_VisualBoard.LoadBoardState(m_BoardStates[m_CurrentBoardStateID]);
        CheckGameStates();
    }

    public void SubmitMove()
    {
        SaveBoardState();

        CheckGameStates();

        SwapTurns();

        if (m_GameState == GameState.EndGame)
            return;

        //If the new player is an AI, let him think!
        if (m_PlayerTypes[(int)m_CurrentPlayer] == PlayerType.AI &&
            m_GameState == GameState.Game)
        {
            AIMove();
        }
    }

    private void CheckGameStates()
    {
        //If black submits a move & we're currently in setup mode, we swap to game mode!
        if (m_GameState == GameState.Setup && m_CurrentPlayer == PlayerColor.Black)
        {
            SetGameState(GameState.Game);
        }

        //If a king is dead, the game is done!
        if (m_GameState == GameState.Game)
        {
            if (m_BoardStates[m_CurrentBoardStateID].IsKingDead(PlayerColor.White))
            {
                Debug.Log("Black won!");
                SetGameState(GameState.EndGame);
            }

            if (m_BoardStates[m_CurrentBoardStateID].IsKingDead(PlayerColor.Black))
            {
                Debug.Log("White won!");
                SetGameState(GameState.EndGame);
            }
        }
    }

    public void UndoMove()
    {
        //2 is a dirty fix, disallow undoing into setup phase
        if (m_CurrentBoardStateID <= 2) return;

        m_CurrentBoardStateID -= 1;
        SwapTurns();
        LoadBoardState();

        //If the game was done, now it's not anymore!
        if (m_GameState == GameState.EndGame)
            SetGameState(GameState.Game);
    }

    public void RedoMove()
    {
        if (m_CurrentBoardStateID >= (m_BoardStates.Count - 1)) return;

        m_CurrentBoardStateID += 1;
        SwapTurns();
        LoadBoardState();
    }

    public void SwapTurns()
    {
        if (m_CurrentPlayer == PlayerColor.White) { m_CurrentPlayer = PlayerColor.Black; }
        else                                      { m_CurrentPlayer = PlayerColor.White; }

        //m_VisualBoard.EnableUnitSelection(m_CurrentPlayer, true);
        //m_VisualBoard.EnableUnitSelection(OtherPlayer(m_CurrentPlayer), false);

        if (m_GameState == GameState.Setup)
        {
            m_VisualBoard.ShowUnits(m_CurrentPlayer, true);
            m_VisualBoard.ShowUnits(OtherPlayer(m_CurrentPlayer), false);
        }

        //Flip the board if required
        if (m_GameMode == GameMode.PassAndPlay && !IsAIPlaying())
        {
            bool flip = (CurrentPlayer == PlayerColor.Black);
            m_VisualBoard.FlipBoard(flip);
        }

        Debug.Log(m_CurrentPlayer.ToString() + "'s turn");
    }

    public void AIMove()
    {
        StartCoroutine(AIMoveRoutine());
    }

    private IEnumerator AIMoveRoutine()
    {
        //Quickly placed into a routine, to mask hickups (& make AI vs AI look sensible)
        yield return new WaitForSeconds(0.2f);

        //Get the current boardstate
        if (m_VisualBoard == null)
            yield return null;

        BoardState currentBoardState = m_VisualBoard.CurrentBoardState;
        currentBoardState.SetCurrentPlayer(m_CurrentPlayer);

        //Calculate all the moves
        DateTime time = DateTime.Now;
        currentBoardState.ProcessAllMoves(m_AIBoardStates);
        DateTime time2 = DateTime.Now;

        double ms = (time2 - time).TotalMilliseconds;
        Debug.Log("Processing all the moves for " + m_AIMoveDepth + " moves took " + ms + "ms");

        //Now we found our best, let's do that
        currentBoardState.ProcessBestMove();

        //Load visually & done!
        m_VisualBoard.LoadBoardState(currentBoardState);
        SubmitMove();

        yield return null;
    }

    public void SetGameState(GameState gameState)
    {
        m_GameState = gameState;

        switch (gameState)
        {
            case GameState.Setup:
                {
                    if (m_ConfirmFormationButton != null) { m_ConfirmFormationButton.SetActive(true); }

                    //m_VisualBoard.EnableUnitSelection(m_CurrentPlayer, true);
                    //m_VisualBoard.EnableUnitSelection(OtherPlayer(m_CurrentPlayer), false);

                    m_VisualBoard.ShowUnits(m_CurrentPlayer, true);
                    m_VisualBoard.ShowUnits(OtherPlayer(m_CurrentPlayer), false);

                    MenuManager.Instance.ShowEndGameMenu(false);

                    Debug.Log("White setup!");
                }
                break;

            case GameState.Game:
                {
                    if (m_ConfirmFormationButton != null) { m_ConfirmFormationButton.SetActive(false); }
                    m_VisualBoard.ShowUnits(m_CurrentPlayer, true);
                    m_VisualBoard.ShowUnits(OtherPlayer(m_CurrentPlayer), true);

                    MenuManager.Instance.ShowEndGameMenu(false);
                }
                break;

            case GameState.EndGame:
                {
                    if (m_ConfirmFormationButton != null) { m_ConfirmFormationButton.SetActive(false); }
                    m_VisualBoard.ShowUnits(m_CurrentPlayer, true);
                    m_VisualBoard.ShowUnits(OtherPlayer(m_CurrentPlayer), true);

                    if (m_EndGameText == null)
                        return;

                    //Set the text
                    string text = "";

                    //If 1 player was a computer, show win/lose
                    PlayerColor winner = PlayerColor.White;

                    if (m_VisualBoard.CurrentBoardState.IsKingDead(PlayerColor.White)) { winner = PlayerColor.Black; }
                    if (m_VisualBoard.CurrentBoardState.IsKingDead(PlayerColor.Black)) { winner = PlayerColor.White; }

                    if (IsAIPlaying())
                    {
                        if (m_PlayerTypes[(int)winner] == PlayerType.Human) { text = "You win!"; }
                        else                                                { text = "You lose!"; }
                    }

                    //If both players where humans or AI's just show the color
                    else
                    {
                        text = winner.ToString() + " wins!";     
                    }

                    m_EndGameText.text = text;

                    //Enable the panel
                    MenuManager.Instance.ShowEndGameMenu(true);
                }
                break;

            default:
                break;
        }
    }

    public void SetGameMode(GameMode gameMode)
    {
        m_GameMode = gameMode;

        //Pass & play is handled at every swap turns
        bool flip = (m_GameMode == GameMode.TabletPlay);
        m_VisualBoard.FlipUnits(PlayerColor.Black, flip);
    }

    public PlayerColor OtherPlayer(PlayerColor player)
    {
        if (player == PlayerColor.White) { return PlayerColor.Black; }
        return PlayerColor.White;
    }

    public bool IsAIPlaying()
    {
        for (int i = 0; i < m_PlayerTypes.Length; ++i)
        {
            if (m_PlayerTypes[i] == PlayerType.AI)
            {
                return true;
            }
        }

        return false;
    }

}
