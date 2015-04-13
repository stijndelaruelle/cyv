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
            for (int mountainID = 0; mountainID < 6; ++mountainID)
            {
                newUnit = new Unit(mountainUnitDefinition, owner, m_Tiles[tempTileId]);
                m_Units.Add(newUnit);
                ++tempTileId;
            }

            //Add a king
            newUnit = new Unit(kingUnitDefinition, owner, null);
            m_Units.Add(newUnit);
            ++tempTileId;

            //Add ...
        }
    }

    public int EvaluateBoard()
    {
        return 0;
    }
}
