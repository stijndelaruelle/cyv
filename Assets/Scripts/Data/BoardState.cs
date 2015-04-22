using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerType
{
    White, //Min
    Black  //Max
}

public class BoardState
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

    private PlayerType m_CurrentPlayer; //Min or Max (only used for AI)

    //Alpha beta pruning
    private int m_Value = 0;
    public int Value
    {
        get { return m_Value; }
    }

    private Move m_BestMove = new Move();

    private int m_Alpha = 0;
    private int m_Beta = 0;

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
        unitDefinitions.Add(new KingUnitDefinition());
        unitDefinitions.Add(new LightHorseUnitDefinition());
        unitDefinitions.Add(new SpearUnitDefinition());
        unitDefinitions.Add(new CrossbowUnitDefinition());

        PlayerType owner = PlayerType.White;
        Unit newUnit = null;

        int tempTileId = 0;

        for (int playerID = 0; playerID < 2; ++playerID)
        {
            if (playerID == 1) owner = PlayerType.Black;

            //Go trough all the unit types
            for (int unitType = 0; unitType < unitDefinitions.Count; ++unitType)
            {
                UnitDefinition unitDefinition = unitDefinitions[unitType];

                //Add the right amount of units
                for (int unitID = 0; unitID < unitDefinition.StartAmount; ++unitID)
                {
                    newUnit = new Unit(this, unitDefinition, owner, m_Tiles[tempTileId]);
                    m_Units.Add(newUnit);
                    ++tempTileId;
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
            if (unit.Owner == m_CurrentPlayer && unit.GetTile() != null)
            {
                value += unit.UnitDefinition.Value;

                //Aggressive AI, prefer our own units less than theirs
                //value -= unit.UnitDefinition.Value + 1;

                //Defensive AI, prefer our own units more than theirs
                //value += 1;
            }
            
            if (unit.Owner != m_CurrentPlayer && unit.GetTile() != null)
            {
                value -= unit.UnitDefinition.Value;
            }
        }

        //Defensive AI, prefer more units (either side)
        //value -= m_Units.Count * 100;

        //If we're min, invert this value
        if (m_CurrentPlayer == PlayerType.White)
        {
            value *= -1;
        }

        return value;
    }

    public void CopyBoard(BoardState otherState)
    {
        m_CurrentPlayer = otherState.m_CurrentPlayer;
        //m_Value = otherState.Value;

        //Loop over all the units
        List<Unit> otherUnits = otherState.Units;
        for (int i = 0; i < otherUnits.Count; ++i)
        {
            //Copy the unit data
            m_Units[i].Copy(otherUnits[i]);
        }
    }

    public void ProcessAllMoves(List<BoardState> boardStates, int id = 0)
    {
        if (id < boardStates.Count)
        {
            //Reset the value to an insane value (the opposite of what we prefer)
            m_Value = int.MaxValue;
            if (m_CurrentPlayer == PlayerType.Black) { m_Value *= -1; }

            List<Move> goodMoves = new List<Move>();
            int ownMovesTillValue = id;

            //Get the next boardstate & make it a copy of this one
            BoardState nextBoardState = boardStates[id];
            ++id;

            //Go trough all the units
            for (int i = 0; i < m_Units.Count; ++i)
            {
                if (m_Units[i].Owner != m_CurrentPlayer)
                    continue;

                //Calculate all the possible moves, as every move generates a new boardstate
                int totalMoves = m_Units[i].CalculateMovecounts();
                
                if (totalMoves == 0)
                    //Cancels out dead units, but when there are no more units alive the value will have the max or min value (not a problem, just be aware)
                    continue;

                for (int moveid = 0; moveid < totalMoves; ++moveid)
                {
                    nextBoardState.CopyBoard(this);
                    nextBoardState.Units[i].ProcessMove(moveid);

                    nextBoardState.SwapCurrentPlayer();
                    nextBoardState.ProcessAllMoves(boardStates, id);

                    //Get the value and make and compare it
                    bool addMove = false;
                    if (m_CurrentPlayer == PlayerType.White && m_Value >= nextBoardState.Value) //min
                    {
                        addMove = true;
                    }

                    if (m_CurrentPlayer == PlayerType.Black && m_Value <= nextBoardState.Value) //max
                    {
                        addMove = true;
                    }

                    if (addMove)
                    {
                        //Determines the min amount of moves to get this value
                        //Later used to resolve a tie
                        int movesTillValue = id;
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
                    }
                }
            }

            //Now we have all the good moves, determine our favourite
            if (goodMoves.Count > 0)
            {
                m_BestMove = goodMoves[0];
                for (int i = 0; i < goodMoves.Count; ++i)
                {
                    bool changeBestMove = false;
                    if (m_CurrentPlayer == PlayerType.White && m_BestMove.value >= goodMoves[i].value) //min
                    {
                        changeBestMove = true;
                    }

                    if (m_CurrentPlayer == PlayerType.Black && m_BestMove.value <= goodMoves[i].value) //max
                    {
                        changeBestMove = true;
                    }

                    if (changeBestMove)
                    {
                        //Which is currently just the one that took the least time
                        if (goodMoves[i].movesTillValue < m_BestMove.movesTillValue)
                        {
                            m_BestMove = goodMoves[i];
                        }
                    }
                }
            }
        }
        else
        {
            //Just calculate the board and that's it!
            m_Value = EvaluateBoard();
        }
    }

    public void ProcessBestMove()
    {
        Units[m_BestMove.unitID].ProcessMove(m_BestMove.moveID);
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

    public void SetCurrentPlayer(PlayerType playerType)
    {
        m_CurrentPlayer = playerType;
    }

    public void SwapCurrentPlayer()
    {
        if (m_CurrentPlayer == PlayerType.White) { m_CurrentPlayer = PlayerType.Black; }
        else                                     { m_CurrentPlayer = PlayerType.White; }
    }
}
