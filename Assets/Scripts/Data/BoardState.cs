using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerColor
{
    White, //Min
    Black  //Max
}

public class BoardState : ThreadedJob
{
    private struct Move
    {
        public Move(int newUnitID, int newMoveID, int newValue, int newMovesTillValue)
        {
            unitID = newUnitID;
            moveID = newMoveID;
            value = newValue;
            movesTillValue = newMovesTillValue;
        }

        public int unitID;
        public int moveID;
        public int value;
        public int movesTillValue;
    }

    public static int DIR_NUM = 12;
    public static int BOARD_SIZE = 6;

    public VoidDelegate OnThreadFinishedEvent;

    //List of units (0-x = white, x-(x+x) = black)
    private List<Unit> m_Units;
    public List<Unit> Units
    {
        get { return m_Units; }
    }

    //List of all the tiles
    private List<Tile> m_Tiles;
    public List<Tile> Tiles
    {
        get { return m_Tiles; }
    }

    private PlayerColor m_CurrentPlayer; //Min or Max (only used for AI)
    public PlayerColor CurrentPlayer
    {
        get { return m_CurrentPlayer; }
    }

    //Alpha beta pruning
    private int m_Value = 0;
    public int Value
    {
        get { return m_Value; }
    }

    private Move m_BestMove = new Move();

    //The last used tiles, for highlighting purposes
    private int m_FromTileID = -1;
    private int m_ToTileID = -1;
    private int m_PromotionTileID = -1;

    //Current thread data
    List<BoardState> m_ThreadBoardStates;
    int m_ThreadAlpha;
    int m_ThreadBeta;
    int m_ThreadId;

    public int FromTileID
    {
        get { return m_FromTileID; }
        set { m_FromTileID = value; }
    }

    public int ToTileID
    {
        get { return m_ToTileID; }
        set { m_ToTileID = value; }
    }

    public int PromotionTileID
    {
        get { return m_PromotionTileID; }
        set { m_PromotionTileID = value; }
    }

    //Functions
    public void GenerateDefaultState(int boardSize)
    {
        //Generate board
        GenerateHexBoard(boardSize);

        //Create all the units
        GenerateUnits();

    }

    private void GenerateHexBoard(int boardSize)
    {
        if (m_Tiles == null) m_Tiles = new List<Tile>();
        m_Tiles.Clear();

        //Calculate board size
        int rowWidth = boardSize;

        //Rows
        int tileID = 0;
        for (int i = 0; i < (boardSize * 2) - 1; ++i)
        {
            //Columns
            for (int j = 0; j < rowWidth; ++j)
            {
                //Create a new tile
                Tile newTile = new Tile(tileID);
                m_Tiles.Add(newTile);
                ++tileID;

                //-----------------------------------------------------------------
                // Manage neighbours backwards (so we are sure they already exist)
                //-----------------------------------------------------------------

                //Bolow a diagram of how the neighbour ID's work (d = diagonal)
                //   11    0d   1
                //   10d   /\   2d 
                //   9    |  |  3
                //   8d    \/   4d 
                //   7     6d   5

                //--------------
                // ORTHOGONAL
                //--------------
                //Left neighbour (9), only exists if you're not the first one in the row
                if (j != 0)
                {
                    int id = (m_Tiles.Count - 2);

                    Tile neigbourTile = m_Tiles[id];
                    newTile.SetNeightbour(9, neigbourTile);
                    neigbourTile.SetNeightbour(3, newTile);
                }

                //Top right corner (1)
                if (i == 0)                                    { /*Nothing*/ }
                else if (j == (rowWidth - 1) && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_Tiles.Count - 1);
                    id -= rowWidth;
                    if (i < boardSize)
                        id += 1; //On the growing side remove an extra id

                    Tile neigbourTile = m_Tiles[id];
                    newTile.SetNeightbour(1, neigbourTile);
                    neigbourTile.SetNeightbour(7, newTile);
                }

                //Top left neightbour (11)
                if (i == 0)                       { /*Nothing*/ }
                else if (j == 0 && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_Tiles.Count - 1);
                    id -= rowWidth;
                    if (i >= boardSize) id -= 1; //On the shrinking side remove an extra id

                    Tile neigbourTile = m_Tiles[id];
                    newTile.SetNeightbour(11, neigbourTile);
                    neigbourTile.SetNeightbour(5, newTile);
                }

                //--------------
                // DIAGONAL
                //--------------

                //Top neighbour (0)
                if (i < 2)
                {
                    /*Nothing*/
                }
                else if (i < boardSize && (j == 0 || j == (rowWidth - 1)))
                {
                    /*Nothing*/
                }
                else
                {
                    //Get id
                    int id = (m_Tiles.Count - 1);
                    id -= rowWidth;

                    if (i > boardSize)       { id -= (rowWidth + 2); }    
                    else if (i < boardSize)  { id -= (rowWidth - 2); }
                    else if (i == boardSize) { id -= (rowWidth + 1); }

                    Tile neigbourTile = m_Tiles[id];
                    newTile.SetNeightbour(0, neigbourTile);
                    neigbourTile.SetNeightbour(6, newTile);
                }

                //Top right diagonal corner (2)
                if (i == 0)                                    { /*Nothing*/ }
                else if (j == (rowWidth - 1))                  { /*Nothing*/ }
                else if (j == (rowWidth - 2) && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_Tiles.Count - 1);
                    id -= rowWidth - 1;
                    if (i < boardSize)
                        id += 1; //On the shrinking side remove an extra id

                    Tile neigbourTile = m_Tiles[id];
                    newTile.SetNeightbour(2, neigbourTile);
                    neigbourTile.SetNeightbour(8, newTile);
                }

                //Top left neightbour (10)
                if (i == 0)                       { /*Nothing*/ }
                else if (j == 0)                  { /*Nothing*/ }
                else if (j == 1 && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_Tiles.Count - 1);
                    id -= rowWidth + 1;
                    if (i >= boardSize) id -= 1; //On the shrinking side remove an extra id

                    Tile neigbourTile = m_Tiles[id];
                    newTile.SetNeightbour(10, neigbourTile);
                    neigbourTile.SetNeightbour(4, newTile);
                }
            }

            //Alter the width & start X coord of the new row
            if (i < (boardSize - 1)) { rowWidth += 1; } //Grow
            else                     { rowWidth -= 1; } //Shrink
        }
    }

    private void GenerateUnits()
    {
        if (m_Units == null) m_Units = new List<Unit>();
        m_Units.Clear();

        //6 Mountains, 1 king, 6 Rabble, 2 crossbows, 2 spears, 2 light horse, 2 catapults, 2 elephants, 2 heavy horse, 1 dragon = 26
        //MountainUnitDefinition mountainUnitDefinition = new MountainUnitDefinition();
        List<UnitDefinition> unitDefinitions = new List<UnitDefinition>();
        unitDefinitions.Add(new MountainUnitDefinition());
        unitDefinitions.Add(new KingUnitDefinition());
        unitDefinitions.Add(new RabbleUnitDefinition());
        unitDefinitions.Add(new LightHorseUnitDefinition());
        unitDefinitions.Add(new SpearUnitDefinition());
        unitDefinitions.Add(new CrossbowUnitDefinition());
        unitDefinitions.Add(new HeavyHorseUnitDefinition());
        unitDefinitions.Add(new ElephantUnitDefinition());
        unitDefinitions.Add(new CatapultUnitDefinition());
        unitDefinitions.Add(new DragonUnitDefinition());

        PlayerColor owner = PlayerColor.White;
        Unit newUnit = null;

        for (int playerID = 0; playerID < 2; ++playerID)
        {
            if (playerID == 1) owner = PlayerColor.Black;

            //Go trough all the unit types
            for (int unitType = 0; unitType < unitDefinitions.Count; ++unitType)
            {
                UnitDefinition unitDefinition = unitDefinitions[unitType];

                //Add the right amount of units
                for (int unitID = 0; unitID < unitDefinition.StartAmount; ++unitID)
                {
                    newUnit = new Unit(this, unitDefinition, owner, null);
                    m_Units.Add(newUnit);

                }
            }
        }
    }

    public int EvaluateBoard()
    {
        //Count up all our unit values
        int value = 0;
        
        foreach (Unit unit in m_Units)
        {
            if (unit.Owner == m_CurrentPlayer)
            {
                if (unit.GetTile() != null)
                {
                    value += unit.UnitDefinition.Value;

                    //Aggressive AI, prefer our own units less than theirs
                    if (GameplayManager.Instance.AIType == AIType.Aggressive)
                    {
                        value -= 1;
                    }

                    //Defensive AI, prefer our own units more than theirs
                    if (GameplayManager.Instance.AIType == AIType.Defensive)
                    {
                        value += 1;
                    }
                }

                //Losing the king gives such a huge failure, it's impossible to ignore
                else if (unit.UnitDefinition.UnitType == UnitType.King && unit.GetTile() == null)
                {
                    value -= 99999999;
                }
            }
            
            if (unit.Owner != m_CurrentPlayer)
            {
                if (unit.GetTile() != null)
                {
                    value -= unit.UnitDefinition.Value;
                }

                //Killing the king gives such a huge boost, it's impossible to ignore
                //Points are bigger than losing your own king, as that simply doesn't matter anymore
                else if (unit.UnitDefinition.UnitType == UnitType.King && unit.GetTile() == null)
                {
                    value += 99999;
                }
            }
        }

        //If we're min, invert this value
        if (m_CurrentPlayer == PlayerColor.White)
        {
            value *= -1;
        }

        return value;
    }

    public void LoadBoard(BoardStateSaveData saveData)
    {
        for (int i = 0; i < m_Units.Count; ++i)
        {
            int tileID = saveData.GetUnitTile(i);
            
            Tile tile = null;
            if (tileID >= 0) { tile = m_Tiles[tileID]; }

            //-2 means not setup, used for AI formations
            if (tileID != -2)
            {
                m_Units[i].SetTile(tile);
            }
        }
    }

    public void CopyBoard(BoardState otherState)
    {
        m_ThreadBoardStates = otherState.m_ThreadBoardStates;
        m_ThreadAlpha = otherState.m_ThreadAlpha;
        m_ThreadBeta = otherState.m_ThreadBeta;
        m_ThreadId = otherState.m_ThreadId;

        m_CurrentPlayer = otherState.m_CurrentPlayer;
        m_FromTileID = otherState.m_FromTileID;
        m_ToTileID = otherState.m_ToTileID;
        m_PromotionTileID = otherState.PromotionTileID;

        //Loop over all the units
        List<Unit> otherUnits = otherState.Units;
        for (int i = 0; i < otherUnits.Count; ++i)
        {
            //Copy the unit data
            m_Units[i].Copy(otherUnits[i]);
        }
    }

    public void ProcessAllMoves(List<BoardState> boardStates, int alpha, int beta, int id = 0)
    {
        //ThreadFunction(boardStates, alpha, alpha, id);
        m_ThreadBoardStates = boardStates;
        m_ThreadAlpha = alpha;
        m_ThreadBeta = beta;
        m_ThreadId = id;

        ThreadFunction();

        if (id == 0)
        {
            OnThreadFinished();
        }

        //if (id != 0)
        //{
        //    ThreadFunction();
        //}
        //else
        //{
        //    StartThread();
        //}
    }

    protected override void ThreadFunction()
    {
        if (m_ThreadId < m_ThreadBoardStates.Count)
        {
            //Reset the value to the worst case scenario
            m_Value = int.MaxValue;
            if (m_CurrentPlayer == PlayerColor.Black) { m_Value *= -1; }

            List<Move> goodMoves = new List<Move>();
            int ownMovesTillValue = m_ThreadId;

            //Get the next boardstate & make it a copy of this one
            BoardState nextBoardState = m_ThreadBoardStates[m_ThreadId];
            ++m_ThreadId;

            //Go trough all the units
            for (int i = 0; i < m_Units.Count; ++i)
            {
                #region alpha-beta pruning
                //Alpha beta pruning
                if (m_CurrentPlayer == PlayerColor.Black)
                {
                    //Our worst case scenario value is higher than the minimizer value above us.
                    //The minimizer will never accept any of our values, so we can quit right here.
                    if (m_Value > m_ThreadBeta)
                    {
                        //Debug.Log("Pruned!");
                        break;
                    }
                }
                else
                {
                    //Our worst case scenario value is lower than the maximizer value above us.
                    //The maximizer will never accept any of our values, so we can quit right here.
                    if (m_Value < m_ThreadAlpha)
                    {
                        //Debug.Log("Pruned!");
                        break;
                    }
                }
                #endregion

                if (m_Units[i].Owner != m_CurrentPlayer)
                    continue;

                //Calculate all the possible moves, as every move generates a new boardstate
                int totalMoves = m_Units[i].CalculateMovecounts();

                if (totalMoves == 0)
                    //Cancels out dead units, but when there are no more units alive the value will have the max or min value (not a problem, just be aware)
                    continue;

                for (int moveid = 0; moveid < totalMoves; ++moveid)
                {
                    #region alpha-beta pruning
                    //Alpha beta pruning
                    if (m_CurrentPlayer == PlayerColor.Black) //Max
                    {
                        //Our worst case scenario value is higher than the minimizer value above us.
                        //The minimizer will never accept any of our values, so we can quit right here.
                        if (m_Value > m_ThreadBeta)
                        {
                            //Debug.Log("Pruned!");
                            break;
                        }
                    }
                    else //Min
                    {
                        //Our worst case scenario value is lower than the maximizer value above us.
                        //The maximizer will never accept any of our values, so we can quit right here.
                        if (m_Value < m_ThreadAlpha)
                        {
                            //Debug.Log("Pruned!");
                            break;
                        }
                    }
                    #endregion

                    nextBoardState.CopyBoard(this);
                    bool doWePromote = nextBoardState.Units[i].ProcessMove(moveid);

                    //if (id == 1)
                    //{
                    //    Debug.Log("");
                    //}

                    #region Promotion
                    if (doWePromote)
                    {
                        //Check all the possible promotion options
                        List<int> promotableUnitIDs = new List<int>();

                        for (int j = 0; j < nextBoardState.m_Units.Count; ++j)
                        {
                            Unit unit = nextBoardState.m_Units[j];

                            if (unit.Owner == m_CurrentPlayer &&
                                unit.UnitDefinition.Tier == 2)
                            {
                                Tile tile = unit.GetTile();
                                if (tile != null)
                                {
                                    promotableUnitIDs.Add(j);
                                }
                            }
                        }

                        if (promotableUnitIDs.Count > 0)
                        {
                            //For each of them process all the moves
                            //TEMP FIX TODO: Just pick a random one for now (otherwise another extra layer)
                            System.Random rand = new System.Random();
                            int randID = rand.Next(0, promotableUnitIDs.Count);

                            Unit promotedUnit = nextBoardState.Units[promotableUnitIDs[randID]];

                            //Find an available upgrade unit
                            for (int j = 0; j < nextBoardState.m_Units.Count; ++j)
                            {
                                Unit unit = nextBoardState.m_Units[j];

                                //Corect type, correct color, and unused!
                                if (unit.UnitDefinition.UnitType == promotedUnit.UnitDefinition.PromotedType &&
                                    unit.Owner == promotedUnit.Owner &&
                                    unit.GetTile() == null)
                                {
                                    //We found an available unit!
                                    //Now swap it with the current one
                                    Tile tile = promotedUnit.GetTile();
                                    promotedUnit.SetTile(null);
                                    unit.SetTile(tile);
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    if (m_ThreadId < m_ThreadBoardStates.Count) { nextBoardState.SwapCurrentPlayer(); }
                    nextBoardState.ProcessAllMoves(m_ThreadBoardStates, m_ThreadAlpha, m_ThreadBeta, m_ThreadId);

                    //Get the value and make and compare it
                    bool addMove = false;
                    if (m_CurrentPlayer == PlayerColor.White && m_Value >= nextBoardState.Value) //min
                    {
                        addMove = true;
                    }

                    if (m_CurrentPlayer == PlayerColor.Black && m_Value <= nextBoardState.Value) //max
                    {
                        addMove = true;
                    }

                    if (addMove)
                    {
                        //Determines the min amount of moves to get this value
                        //Later used to resolve a tie
                        int movesTillValue = m_ThreadId;
                        if (m_Value == int.MaxValue || m_Value == int.MinValue) { m_Value = EvaluateBoard(); }

                        //If the value is exactly the same as ours, then we didn't even need that extra move
                        if (m_Value == nextBoardState.Value)
                        {
                            movesTillValue = ownMovesTillValue;
                        }

                        //If the value is different, we did. (Clearing is done as this move is at this point 100% better)
                        else
                        {
                            goodMoves.Clear();
                        }

                        m_Value = nextBoardState.Value;
                        goodMoves.Add(new Move(i, moveid, nextBoardState.Value, movesTillValue));

                        //Set the alpha & beta
                        if (m_CurrentPlayer == PlayerColor.Black)
                        {
                            m_ThreadAlpha = m_Value;
                        }
                        else
                        {
                            m_ThreadBeta = m_Value;
                        }
                    }
                }
            }

            //if (id == 1)
            //{
            //    Debug.Log("");
            //}

            #region Best Move Decision
            //Now we have all the good moves, determine our favourite
            List<Move> bestMoves = new List<Move>();
            if (goodMoves.Count > 0)
            {
                m_BestMove = goodMoves[0];
                for (int i = 0; i < goodMoves.Count; ++i)
                {
                    bool addBestMove = false;
                    if (m_CurrentPlayer == PlayerColor.White && m_BestMove.value >= goodMoves[i].value) //min
                    {
                        addBestMove = true;
                    }

                    if (m_CurrentPlayer == PlayerColor.Black && m_BestMove.value <= goodMoves[i].value) //max
                    {
                        addBestMove = true;
                    }

                    if (addBestMove)
                    {
                        //Which is currently just the one that took the least time
                        if (goodMoves[i].movesTillValue < m_BestMove.movesTillValue)
                        {
                            bestMoves.Clear();
                        }

                        bestMoves.Add(goodMoves[i]);
                    }
                }
            }

            //Now we have the absolute best moves
            if (bestMoves.Count > 0)
            {
                //Take whatever we have
                if (bestMoves.Count == 1) { m_BestMove = bestMoves[0]; }
                else
                {
                    List<Move> randomMovePool = new List<Move>();
                    bool mustMoveKing = true;
                    for (int i = 0; i < bestMoves.Count; ++i)
                    {
                        if (m_Units[bestMoves[i].unitID].UnitDefinition.UnitType != UnitType.King)
                        {
                            randomMovePool.Add(bestMoves[i]);
                            mustMoveKing = false;
                        }
                    }

                    //Randomise
                    if (!mustMoveKing)
                    {
                        //Only choose between non king units
                        int randID = Random.Range(0, randomMovePool.Count);
                        m_BestMove = randomMovePool[randID];
                    }
                    else
                    {
                        //The king is required so let's go
                        int randID = Random.Range(0, bestMoves.Count);
                        m_BestMove = bestMoves[randID];
                    }
                }
            }
            #endregion
        }
        else
        {
            //Just calculate the board and that's it!
            m_Value = EvaluateBoard();
        }
    }

    protected override void OnThreadFinished()
    {
        // This is executed by the Unity main thread when the job is finished
        if (OnThreadFinishedEvent != null)
            OnThreadFinishedEvent();
    }

    public void ProcessBestMove()
    {
        //Only called by AI!
        m_FromTileID = Units[m_BestMove.unitID].GetTile().ID;

        bool doWePromote = Units[m_BestMove.unitID].ProcessMove(m_BestMove.moveID);

        m_ToTileID = Units[m_BestMove.unitID].GetTile().ID;

        #region Promotion

        PromotionTileID = -1;
        if (doWePromote)
        {
            //Check all the possible promotion options
            List<int> promotableUnitIDs = new List<int>();

            for (int j = 0; j < m_Units.Count; ++j)
            {
                Unit unit = m_Units[j];

                if (unit.Owner == m_CurrentPlayer &&
                    unit.UnitDefinition.Tier == 2)
                {
                    Tile tile = unit.GetTile();
                    if (tile != null)
                    {
                        promotableUnitIDs.Add(j);
                    }
                }
            }

            if (promotableUnitIDs.Count > 0)
            {
                //For each of them process all the moves
                //TEMP FIX TODO: Just pick a random one for now (otherwise another extra layer)
                int randID = Random.Range(0, promotableUnitIDs.Count);
                Unit promotedUnit = Units[promotableUnitIDs[randID]];

                //Find an available upgrade unit
                for (int j = 0; j < m_Units.Count; ++j)
                {
                    Unit unit = m_Units[j];

                    //Corect type, correct color, and unused!
                    if (unit.UnitDefinition.UnitType == promotedUnit.UnitDefinition.PromotedType &&
                        unit.Owner == promotedUnit.Owner &&
                        unit.GetTile() == null)
                    {
                        //We found an available unit!
                        //Now swap it with the current one
                        Tile tile = promotedUnit.GetTile();
                        promotedUnit.SetTile(null);
                        unit.SetTile(tile);

                        //Visually show the upgraded tile
                        PromotionTileID = tile.ID;
                        break;
                    }
                }
            }
        }
        #endregion

        //If a promotion happened set the tile here
        //m_PromotionTileID
    }

    public Tile GetTile(int tileID)
    {
        if (tileID < 0 || tileID >= m_Tiles.Count)
            return null;

        return m_Tiles[tileID];
    }

    public Unit GetUnit(int unitID)
    {
        if (unitID < 0 || unitID >= m_Units.Count)
            return null;

        return m_Units[unitID];
    }

    public void SetCurrentPlayer(PlayerColor playerColor)
    {
        m_CurrentPlayer = playerColor;
    }

    public void SwapCurrentPlayer()
    {
        if (m_CurrentPlayer == PlayerColor.White) { m_CurrentPlayer = PlayerColor.Black; }
        else                                      { m_CurrentPlayer = PlayerColor.White; }
    }

    public bool IsKingDead(PlayerColor playerColor)
    {
        foreach (Unit unit in m_Units)
        {
            if (unit.Owner == playerColor && unit.UnitDefinition.UnitType == UnitType.King)
            {
                if (unit.GetTile() == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool HasOnlyKing(PlayerColor playerColor)
    {
        foreach (Unit unit in m_Units)
        {
            //Check if any other character are alive
            if (unit.Owner == playerColor &&
                unit.UnitDefinition.UnitType != UnitType.King &&
                unit.UnitDefinition.UnitType != UnitType.Mountain &&
                unit.GetTile() != null)
            {
                return false;
            }
        }

        return true;
    }

    public bool PlacedAllUnits(PlayerColor playerColor)
    {
        foreach (Unit unit in m_Units)
        {
            if (unit.Owner == playerColor)
            {
                if (unit.GetTile() == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void Update()
    {
        UpdateThread();
    }
}
