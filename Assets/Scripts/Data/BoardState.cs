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
    //List of units (0-25 = white, 26-51 = black)
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

    private PlayerType m_CurrentPlayerType; //Min or Max

    //Alpha beta pruning
    private int m_Value = 0;
    public int Value
    {
        get { return m_Value; }
    }

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
        int prevRowWdith = boardSize;

        //Rows
        for (int i = 0; i < (boardSize * 2) - 1; ++i)
        {
            //Columns
            for (int j = 0; j < rowWidth; ++j)
            {
                //Create a new tile
                Tile newTile = new Tile();
                m_Tiles.Add(newTile);

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
        MountainUnitDefinition mountainUnitDefinition = new MountainUnitDefinition();
        KingUnitDefinition     kingUnitDefinition     = new KingUnitDefinition();


        PlayerType owner = PlayerType.White;
        Unit newUnit = null;

        int tempTileId = 0;

        for (int playerID = 0; playerID < 2; ++playerID)
        {
            if (playerID == 1) owner = PlayerType.Black;

            //Add mountains
            //for (int unitID = 0; unitID < mountainUnitDefinition.StartAmount; ++unitID)
            //{
            //    newUnit = new Unit(mountainUnitDefinition, owner, m_Tiles[tempTileId]);
            //    m_Units.Add(newUnit);
            //    ++tempTileId;
            //}

            //Add a king
            for (int unitID = 0; unitID < kingUnitDefinition.StartAmount; ++unitID)
            {
                newUnit = new Unit(kingUnitDefinition, owner, m_Tiles[tempTileId]);
                m_Units.Add(newUnit);
                ++tempTileId;
            }

            //Add ...
        }
    }

    public int EvaluateBoard()
    {
        //Count up all our unit values
        int value = 0;
        foreach (Unit unit in m_Units)
        {
            if (unit.Owner == m_CurrentPlayerType)
            {
                value += unit.UnitDefinition.Value;
            }
            else
            {
                value -= unit.UnitDefinition.Value;
            }
        }

        //If we're min, invert this value
        if (m_CurrentPlayerType == PlayerType.White)
        {
            value *= -1;
        }

        return value;
    }

    public void CopyUnits(BoardState otherState)
    {
        //Loop over all the units
        List<Unit> otherUnits = otherState.Units;
        List<Tile> otherTiles = otherState.Tiles;

        //Loop over all the tiles
        for (int i = 0; i < otherUnits.Count; ++i)
        {
            //Copy the unit data
            m_Units[i].Copy(otherUnits[i]);

            //Set the tile
            Tile tile = otherUnits[i].GetTile();

            if (tile != null)
            {
                //Find the index of the tile this unit was standing on
                int tileIndex = otherTiles.IndexOf(tile);

                //And use our own tile on that location
                m_Units[i].SetTile(m_Tiles[tileIndex]);
            }
        }
    }

    public void ProcessAllMoves(List<BoardState> boardStates)
    {
        if (boardStates.Count > 0)
        {
            //Get the next boardstate & make it a copy of this one
            BoardState nextBoardState = boardStates[boardStates.Count - 1];
            boardStates.RemoveAt(boardStates.Count - 1);

            //Go trough all the units
            for (int i = 0; i < m_Units.Count; ++i)
            {
                //Calculate all the possible moves, as every move generates a new boardstate
                int totalMoves = m_Units[i].CalculateMovecounts();

                for (int moveid = 0; moveid < totalMoves; ++moveid)
                {
                    nextBoardState.CopyUnits(this);
                    nextBoardState.Units[i].ProcessMove(moveid);
                    //nextBoardState.ProcessAllMoves(boardStates);

                    //Get the value and make and compare it
                    //NOTE: CURRENTLY EQUALLY GOOD MOVES ARE IGNORED!!!!
                    //if (m_CurrentPlayerType == PlayerType.White && m_Value > nextBoardState.Value) //min
                    //{
                    //    m_Value = nextBoardState.Value;
                    //}

                    //if (m_CurrentPlayerType == PlayerType.Black && m_Value < nextBoardState.Value) //max
                    //{
                    //    m_Value = nextBoardState.Value;
                    //}
                }
            }
        }
        else
        {
            //Just calculate the board and that's it!
            EvaluateBoard();
        }

    }
}
