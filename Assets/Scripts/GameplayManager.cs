using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public enum GameState
{
    Setup,
    Game,
    EndGame,
    Promotion //While promoting
}

public enum PlayerType
{
    Human,
    AI
}

public enum AIType
{
    Standard,
    Aggressive,
    Defensive
}

public enum GameMode
{
    PassAndPlay, //Board flips turns after every move (unless AI is involved)
    MirroredPlay   //Black units are flipped at the beginning of the game
}

//Delegates
public delegate void VoidDelegate();
public delegate void PlayerColorDelegate(PlayerColor playerColor);
public delegate void VisualUnitDelegate(VisualUnit visualUnit);
public delegate void GameStateDelegate(GameState newGameState, GameState previousGameState);

public class NewGameSetup
{
    public PlayerType m_WhitePlayerType;
    public PlayerType m_BlackPlayerType;
    public int m_AIDifficulty;
    public AIType m_AIType;
}

public class GameplayManager : MonoBehaviour
{
    //------------------
    // Events
    //------------------
    public VoidDelegate OnNewGame;
    public GameStateDelegate OnChangeGameState;
    public PlayerColorDelegate OnChangePlayer;
    public VoidDelegate OnNewGameSetupChange;

    //------------------
    // Datamembers
    //------------------
    [SerializeField]
    private VisualBoard m_VisualBoard = null;

    [SerializeField]
    private int m_AIMoveDepth = 3;

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

    private GameMode m_GameMode = GameMode.MirroredPlay;
    public GameMode GameMode
    {
        get { return m_GameMode; }
    }

    private AIType m_AIType = AIType.Standard;
    public AIType AIType
    {
        get { return m_AIType; }
    }

    public PlayerType CurrentPlayerType
    {
        get { return m_PlayerTypes[(int)m_CurrentPlayer]; }
    }

    private bool m_AlreadyFinishedGame = false;

    //Setting up new game
    private NewGameSetup m_NewGameSetup;
    public NewGameSetup NewGameSetup
    {
        get { return m_NewGameSetup; }
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
        m_AIBoardStates = new List<BoardState>();

        //Generate all our needed boardstates
        for (int i = 0; i < m_AIMoveDepth; ++i)
        {
            BoardState boardState = new BoardState();
            boardState.GenerateDefaultState(BoardState.BOARD_SIZE);

            m_AIBoardStates.Add(boardState);
        }

        m_NewGameSetup = new NewGameSetup();
    }

    public void NewGame()
    {
        NewGame(m_NewGameSetup.m_WhitePlayerType, m_NewGameSetup.m_BlackPlayerType, m_NewGameSetup.m_AIDifficulty, m_NewGameSetup.m_AIType);
        SetGameMode(m_GameMode);
    }

    public void NewGame(PlayerType playerType1, PlayerType playerType2, int difficulty, AIType AIType)
    {
        if (OnNewGame != null)
            OnNewGame();

        //Set the playertypes
        m_PlayerTypes[0] = playerType1;
        m_PlayerTypes[1] = playerType2;
        m_CurrentPlayer = PlayerColor.White;
        m_AlreadyFinishedGame = false;

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

        //Set AI depth
        m_AIMoveDepth = difficulty + 2;

        //We have too many board states
        if (m_AIMoveDepth < m_AIBoardStates.Count)
        {
            int count = m_AIBoardStates.Count - m_AIMoveDepth;
            m_AIBoardStates.RemoveRange(m_AIMoveDepth, count);
        }

        //We have too few boardstates
        if (m_AIMoveDepth > m_AIBoardStates.Count)
        {
            int count = m_AIMoveDepth - m_AIBoardStates.Count;
            for (int i = 0; i < count; ++i)
            {
                BoardState newBoardState = new BoardState();
                newBoardState.GenerateDefaultState(BoardState.BOARD_SIZE);

                m_AIBoardStates.Add(newBoardState);
            }
        }

        //Set the AI Type
        m_AIType = AIType;

        SaveBoardState();

        //Analytics
        long value = 0;

        //0 = both human
        //1 = white player, black AI
        //2 = white AI, black player,
        //3 = both Ai
        if (m_PlayerTypes[0] == PlayerType.Human && m_PlayerTypes[1] == PlayerType.Human) { value = 0; }
        if (m_PlayerTypes[0] == PlayerType.Human && m_PlayerTypes[1] == PlayerType.AI) { value = 1; }
        if (m_PlayerTypes[0] == PlayerType.AI && m_PlayerTypes[1] == PlayerType.Human) { value = 2; }
        if (m_PlayerTypes[0] == PlayerType.AI && m_PlayerTypes[1] == PlayerType.AI) { value = 3; }

        AnalyticsManager.Instance.LogEvent("Default", "New Game", "Started a new game.", value);

        //If the AI has the first move, let him do so!
        if (m_PlayerTypes[0] == PlayerType.AI)
        {
            AIMove();
        }

        m_VisualBoard.CurrentBoardState.OnThreadFinishedEvent = null;
        m_VisualBoard.CurrentBoardState.OnThreadFinishedEvent += OnAILogicFinished;
    }

    public void HighlightSetupZone(bool enable)
    {
        m_VisualBoard.HighlightSetupZone(m_CurrentPlayer, enable);
    }

    public void SaveBoardState()
    {
        //Save the current board
        if (m_PlayerTypes[(int)m_CurrentPlayer] != PlayerType.AI) //Not required and skrews up the promotion unit indicator
        {
            m_VisualBoard.SaveBoardState();
        }

        //Clear all the extra's
        if (m_CurrentBoardStateID < m_BoardStates.Count - 1)
        {
            int count = m_BoardStates.Count - m_CurrentBoardStateID - 1;
            m_BoardStates.RemoveRange(m_CurrentBoardStateID + 1, count);

            //Analytics
            AnalyticsManager.Instance.LogEvent("Default", "Rethink", "Undid a move, and made another one.", 0);
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
        StartCoroutine(SubmitMoveRoutine());
    }

    public IEnumerator SubmitMoveRoutine()
    {
        //Wait until all units have stopped animating!
        while (m_VisualBoard.AreUnitsAnimating())
        {
            yield return new WaitForSeconds(0.1f);
        }

        SaveBoardState();

        CheckGameStates();

        SwapTurns();

        //Load the current boardstate (AI changed it without visually showing, non AI also has to show the last move)
        m_VisualBoard.LoadBoardState(m_VisualBoard.CurrentBoardState);

        if (m_GameState == GameState.EndGame)
            yield return null;

        //If the new player is an AI, let him think!
        if (m_PlayerTypes[(int)m_CurrentPlayer] == PlayerType.AI)
        {
            AIMove();
        }

        AnalyticsManager.Instance.LogEvent("Default", "Moved", "Did a move.", m_CurrentBoardStateID);
    }

    public void CheckGameStates()
    {
        //If black submits a move & we're currently in setup mode, we swap to game mode!
        if (m_GameState == GameState.Setup && m_CurrentPlayer == PlayerColor.Black)
        {
            SetGameState(GameState.Game);
            return;
        }

        //If a king is dead, the game is done!
        if (m_GameState == GameState.Game || m_GameState == GameState.EndGame)
        {
            if (m_BoardStates[m_CurrentBoardStateID].IsKingDead(PlayerColor.White) ||
                m_BoardStates[m_CurrentBoardStateID].HasOnlyKing(PlayerColor.White))
            {
                Debug.Log("Red won!");
                SetGameState(GameState.EndGame);
                return;
            }

            if (m_BoardStates[m_CurrentBoardStateID].IsKingDead(PlayerColor.Black) ||
                m_BoardStates[m_CurrentBoardStateID].HasOnlyKing(PlayerColor.Black))
            {
                Debug.Log("White won!");
                SetGameState(GameState.EndGame);
                return;
            }
        }
    }

    public void UndoMove()
    {
        //2 is a dirty fix, disallow undoing into setup phase
        if (m_CurrentBoardStateID <= 2) return;

        //Another dirty fix
        if (m_GameState == GameState.Promotion) return;

        //If the game was done, now it's not anymore!
        if (m_GameState == GameState.EndGame)
            SetGameState(GameState.Game);

        m_CurrentBoardStateID -= 1;
        SwapTurns();
        LoadBoardState();

        //Cheap fix, otherwise after promoting & undoing we get promoted units in the bank.
        m_VisualBoard.ShowUnits(PlayerColor.White, true);
        m_VisualBoard.ShowUnits(PlayerColor.Black, true);
    }

    public void RedoMove()
    {
        if (m_CurrentBoardStateID >= (m_BoardStates.Count - 1)) return;
        if (m_GameState == GameState.Promotion) return;

        m_CurrentBoardStateID += 1;
        SwapTurns();
        LoadBoardState();
    }

    public void SwapTurns()
    {
        if (m_CurrentPlayer == PlayerColor.White) { m_CurrentPlayer = PlayerColor.Black; }
        else                                      { m_CurrentPlayer = PlayerColor.White; }

        if (OnChangePlayer != null)
            OnChangePlayer(m_CurrentPlayer);

        if (m_GameState == GameState.Setup)
        {
            m_VisualBoard.ShowUnits(m_CurrentPlayer, true);

            if (m_PlayerTypes[(int)m_CurrentPlayer] != PlayerType.AI)
            {
                m_VisualBoard.ShowUnits(OtherPlayer(m_CurrentPlayer), false);
            }
        }

        //Flip the board if required
        if (m_GameMode == GameMode.PassAndPlay && NumAIPlayers() == 0)
        {
            bool flip = (CurrentPlayer == PlayerColor.Black);
            m_VisualBoard.FlipBoard(flip);
        }

        Debug.Log(m_CurrentPlayer.ToString() + "'s turn");
    }

    public void AIMove()
    {
        if (m_GameState == GameState.Game)
        {
            StartCoroutine(AIMoveLogicRoutine());
        }
        else
        {
            //Do the setup phase for the AI

            //Set player id
            BoardState currentBoardState = m_VisualBoard.CurrentBoardState;
            currentBoardState.SetCurrentPlayer(m_CurrentPlayer);

            //Load some test data
            int randSetup = UnityEngine.Random.Range(0, 100);
            int setupID = 0;
            if (randSetup > 50)
                setupID = 1;

            if (m_CurrentPlayer == PlayerColor.White)
            {
                switch (m_AIType)
                {
                    case AIType.Standard:
                    {
                        if (setupID == 0)
                        {
                            BoardStateSaveDataWhiteSymmetric1 saveData = new BoardStateSaveDataWhiteSymmetric1();
                            currentBoardState.LoadBoard(saveData);
                        }
                        else
                        {
                            BoardStateSaveDataWhiteSymmetric2 saveData = new BoardStateSaveDataWhiteSymmetric2();
                            currentBoardState.LoadBoard(saveData);
                        }
                        break;
                    }

                    case AIType.Aggressive:
                    {
                        if (setupID == 0)
                        {
                            BoardStateSaveDataWhiteAggressive1 saveData = new BoardStateSaveDataWhiteAggressive1();
                            currentBoardState.LoadBoard(saveData);
                        }
                        else
                        {
                            BoardStateSaveDataWhiteAggressive2 saveData = new BoardStateSaveDataWhiteAggressive2();
                            currentBoardState.LoadBoard(saveData);
                        }
                        break;
                    }

                    case AIType.Defensive:
                    {
                        if (setupID == 0)
                        {
                            BoardStateSaveDataWhiteDefensive1 saveData = new BoardStateSaveDataWhiteDefensive1();
                            currentBoardState.LoadBoard(saveData);
                        }
                        else
                        {
                            BoardStateSaveDataWhiteDefensive2 saveData = new BoardStateSaveDataWhiteDefensive2();
                            currentBoardState.LoadBoard(saveData);
                        }
                        break;
                    }

                    default:
                    {
                        if (setupID == 0)
                        {
                            BoardStateSaveDataWhiteSymmetric1 saveData = new BoardStateSaveDataWhiteSymmetric1();
                            currentBoardState.LoadBoard(saveData);
                        }
                        else
                        {
                            BoardStateSaveDataWhiteSymmetric2 saveData = new BoardStateSaveDataWhiteSymmetric2();
                            currentBoardState.LoadBoard(saveData);
                        }
                        break;
                    }
                }

            }
            else
            {
                switch (m_AIType)
                {
                    case AIType.Standard:
                        {
                            if (setupID == 0)
                            {
                                BoardStateSaveDataBlackSymmetric1 saveData = new BoardStateSaveDataBlackSymmetric1();
                                currentBoardState.LoadBoard(saveData);
                            }
                            else
                            {
                                BoardStateSaveDataBlackSymmetric2 saveData = new BoardStateSaveDataBlackSymmetric2();
                                currentBoardState.LoadBoard(saveData);
                            }
                            break;
                        }

                    case AIType.Aggressive:
                        {
                            if (setupID == 0)
                            {
                                BoardStateSaveDataBlackAggressive1 saveData = new BoardStateSaveDataBlackAggressive1();
                                currentBoardState.LoadBoard(saveData);
                            }
                            else
                            {
                                BoardStateSaveDataBlackAggressive2 saveData = new BoardStateSaveDataBlackAggressive2();
                                currentBoardState.LoadBoard(saveData);
                            }
                            break;
                        }

                    case AIType.Defensive:
                        {
                            if (setupID == 0)
                            {
                                BoardStateSaveDataBlackDefensive1 saveData = new BoardStateSaveDataBlackDefensive1();
                                currentBoardState.LoadBoard(saveData);
                            }
                            else
                            {
                                BoardStateSaveDataBlackDefensive2 saveData = new BoardStateSaveDataBlackDefensive2();
                                currentBoardState.LoadBoard(saveData);
                            }
                            break;
                        }

                    default:
                        {
                            if (setupID == 0)
                            {
                                BoardStateSaveDataBlackSymmetric1 saveData = new BoardStateSaveDataBlackSymmetric1();
                                currentBoardState.LoadBoard(saveData);
                            }
                            else
                            {
                                BoardStateSaveDataBlackSymmetric2 saveData = new BoardStateSaveDataBlackSymmetric2();
                                currentBoardState.LoadBoard(saveData);
                            }
                            break;
                        }
                }
            }

            //Show visually & Done!
            m_VisualBoard.LoadBoardState(m_VisualBoard.CurrentBoardState);
            SubmitMove();
        }
    }

    private IEnumerator AIMoveLogicRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        AIMoveLogic();
    }

    private void AIMoveLogic()
    {
        //Get the current boardstate
        if (m_VisualBoard == null)
            return;

        BoardState currentBoardState = m_VisualBoard.CurrentBoardState;
        currentBoardState.SetCurrentPlayer(m_CurrentPlayer);

        //Calculate all the moves
        currentBoardState.ProcessAllMoves(m_AIBoardStates, int.MinValue, int.MaxValue, 0); //Runs on a separat thread
    }

    private void OnAILogicFinished()
    {
        //Now we found our best, let's do that
        BoardState currentBoardState = m_VisualBoard.CurrentBoardState;
        currentBoardState.ProcessBestMove();

        //Play the move sound!
        AudioManager.Instance.PlaySound(AudioManager.SoundType.DropUnit);

        //Show visually & Done!
        m_VisualBoard.LoadBoardState(m_VisualBoard.CurrentBoardState);
        SubmitMove();
    }

    public void SetGameState(GameState gameState)
    {
        if (OnChangeGameState != null && gameState != m_GameState)
            OnChangeGameState(gameState, m_GameState);

        m_GameState = gameState;

        switch (gameState)
        {
            case GameState.Setup:
                {
                    m_VisualBoard.ShowUnits(m_CurrentPlayer, true);
                    m_VisualBoard.ShowUnits(OtherPlayer(m_CurrentPlayer), true);

                    MenuManager.Instance.ShowEndGameMenu(false);

                    Debug.Log("White setup!");
                }
                break;

            case GameState.Game:
                {
                    m_VisualBoard.ShowUnits(m_CurrentPlayer, true);
                    m_VisualBoard.ShowUnits(OtherPlayer(m_CurrentPlayer), true);

                    MenuManager.Instance.ShowEndGameMenu(false);
                }
                break;

            case GameState.EndGame:
                {
                    m_VisualBoard.ShowUnits(m_CurrentPlayer, true);
                    m_VisualBoard.ShowUnits(OtherPlayer(m_CurrentPlayer), true);

                    if (m_EndGameText == null)
                        return;

                    //Set the text
                    string text = "";

                    //If 1 player was a computer, show win/lose
                    PlayerColor winner = PlayerColor.White;
                    String strWinner = "WHITE";

                    if (m_VisualBoard.CurrentBoardState.IsKingDead(PlayerColor.White) ||
                        m_VisualBoard.CurrentBoardState.HasOnlyKing(PlayerColor.White))
                    {
                        winner = PlayerColor.Black;
                        strWinner = "CRIMSON";
                    }

                    if (m_VisualBoard.CurrentBoardState.IsKingDead(PlayerColor.Black) ||
                        m_VisualBoard.CurrentBoardState.HasOnlyKing(PlayerColor.Black))
                    {
                        winner = PlayerColor.White;
                        strWinner = "WHITE";
                    }

                    if (NumAIPlayers() == 1)
                    {
                        if (m_PlayerTypes[(int)winner] == PlayerType.Human) { text = "VICTORY!"; }
                        else                                                { text = "DEFEAT"; }
                    }

                    //If both players where humans or AI's just show the color
                    else
                    {
                        text = strWinner + " WINS!";     
                    }

                    m_EndGameText.text = text;

                    //Enable the panel
                    MenuManager.Instance.ShowEndGameMenu(true);

                    if (!m_AlreadyFinishedGame)
                    {
                        AnalyticsManager.Instance.LogEvent("Default", "End Game", "The game ended.", (long)winner);
                    }

                    m_AlreadyFinishedGame = true;
                }
                break;

            case GameState.Promotion:
                break;

            default:
                break;
        }
    }

    public void SetGameMode(GameMode gameMode)
    {
        m_GameMode = gameMode;

        //Pass & play is handled at every swap turns
        if (m_GameMode == GameMode.MirroredPlay && NumAIPlayers() == 0)
        {
            m_VisualBoard.FlipBoard(false);
            m_VisualBoard.FlipUnits(PlayerColor.Black, true);
        }
    }

    public PlayerColor OtherPlayer(PlayerColor player)
    {
        if (player == PlayerColor.White) { return PlayerColor.Black; }
        return PlayerColor.White;
    }

    public int NumAIPlayers()
    {
        int numAIPlayers = 0;
        for (int i = 0; i < m_PlayerTypes.Length; ++i)
        {
            if (m_PlayerTypes[i] == PlayerType.AI)
            {
                ++numAIPlayers;
            }
        }

        return numAIPlayers;
    }

    public bool PlacedAllUnits()
    {
        return m_VisualBoard.PlacedAllUnits(m_CurrentPlayer);
    }

    void Update()
    {
        if (m_VisualBoard.CurrentBoardState == null)
            return;

        m_VisualBoard.CurrentBoardState.Update();
    }

    public void UpdateNewGameSetup()
    {
        if (OnNewGameSetupChange != null)
            OnNewGameSetupChange();
    }
}
