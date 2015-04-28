using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VisualBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject m_TilePrefab = null;

    [SerializeField]
    private GameObject[] m_UnitPrefab = null;

    //The reserve tiles (where dead / unused units go)
    [SerializeField]
    private VisualTile[] m_ReserveVisualTiles = new VisualTile[2];

    [SerializeField]
    private Color[] m_Colors;

    private BoardState m_CurrentBoardState = null;
    public BoardState CurrentBoardState
    {
        get { return m_CurrentBoardState; }
    }

    //Not sure why we need this
    private List<VisualTile> m_VisualTiles;
    private List<VisualUnit> m_VisualUnits;

    private void Awake()
    {
        for (int i = 0; i < 2; ++i)
        {
            if (m_ReserveVisualTiles[i] == null)
            {
                Debug.LogError("VisualBoard doesn't have a valid reserve tile at slot: " + i);
            }
            else
            {
                m_ReserveVisualTiles[i].SetID(-1); //-1 means invalid
            }
        }

        GenerateHexBoard(BoardState.BOARD_SIZE);
        GenerateUnits();
    }

    private void GenerateHexBoard(int boardSize)
    {
        if (m_VisualTiles == null) m_VisualTiles = new List<VisualTile>();
        m_VisualTiles.Clear();

        float width = 44;
        float height = 38;

        GameObject obj = null;
        RectTransform rectTransform = null;

        //Calculate board size
        int rowWidth = boardSize;
        float startX = -(width * (boardSize * 0.5f)) + (width / 2);
        float startY = height * (boardSize - 1);
        int startColor = 0;

        //Rows
        for (int i = 0; i < (boardSize * 2) - 1; ++i)
        {
            float x = startX;
            int color = startColor;

            //Columns
            for (int j = 0; j < rowWidth; ++j)
            {
                obj = GameObject.Instantiate(m_TilePrefab) as GameObject;

                rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.SetParent(gameObject.GetComponent<RectTransform>()); //Parent this to us
                rectTransform.anchoredPosition = new Vector2(x, startY);
                rectTransform.localScale = new Vector2(1.0f, 1.0f); //Always resets for some reason

                VisualTile newTile = obj.GetComponent<VisualTile>();
                newTile.SetColor(m_Colors[color]);
                newTile.SetID(m_VisualTiles.Count);
                m_VisualTiles.Add(newTile);

                x += width;

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
                    int id = (m_VisualTiles.Count - 2);

                    VisualTile neigbourTile = m_VisualTiles[id];
                    newTile.SetNeightbour(9, neigbourTile);
                    neigbourTile.SetNeightbour(3, newTile);
                }

                //Top right corner (1)
                if (i == 0)                                    { /*Nothing*/ }
                else if (j == (rowWidth - 1) && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth;
                    if (i < boardSize)
                        id += 1; //On the growing side remove an extra id

                    VisualTile neigbourTile = m_VisualTiles[id];
                    newTile.SetNeightbour(1, neigbourTile);
                    neigbourTile.SetNeightbour(7, newTile);
                }

                //Top left neightbour (11)
                if (i == 0)                       { /*Nothing*/ }
                else if (j == 0 && i < boardSize) { /*Nothing*/ }
                else
                {
                    //Get id
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth;
                    if (i >= boardSize) id -= 1; //On the shrinking side remove an extra id

                    VisualTile neigbourTile = m_VisualTiles[id];
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
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth;

                    if (i > boardSize)       { id -= (rowWidth + 2); }    
                    else if (i < boardSize)  { id -= (rowWidth - 2); }
                    else if (i == boardSize) { id -= (rowWidth + 1); }

                    VisualTile neigbourTile = m_VisualTiles[id];
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
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth - 1;
                    if (i < boardSize)
                        id += 1; //On the shrinking side remove an extra id

                    VisualTile neigbourTile = m_VisualTiles[id];
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
                    int id = (m_VisualTiles.Count - 1);
                    id -= rowWidth + 1;
                    if (i >= boardSize) id -= 1; //On the shrinking side remove an extra id

                    VisualTile neigbourTile = m_VisualTiles[id];
                    newTile.SetNeightbour(10, neigbourTile);
                    neigbourTile.SetNeightbour(4, newTile);
                }

                //-------------------
                // Increase colour
                //-------------------
                color += 1;
                if (color >= m_Colors.Length) color = 0;
            }

            //Alter the width & start X coord of the new row
            if (i < (boardSize - 1))
            {
                startX -= (width * 0.5f);

                rowWidth += 1;
                startColor += 1;
                if (startColor >= m_Colors.Length) startColor = 0;
            }
            else
            {
                startX += (width * 0.5f);
                rowWidth -= 1;

                startColor -= 1;
                if (startColor < 0) startColor = m_Colors.Length - 1;
            }

            //Alter the y coord of the next row
            startY -= height;
        }

        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 40.0f);
    }

    private void GenerateUnits()
    {
        if (m_VisualUnits == null) m_VisualUnits = new List<VisualUnit>();
        m_VisualUnits.Clear();

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

        for (int playerID = 0; playerID < 2; ++playerID)
        {
            for (int unitType = 0; unitType < unitDefinitions.Count; ++unitType)
            {
                UnitDefinition unitDefinition = unitDefinitions[unitType];

                for (int unitID = 0; unitID < unitDefinition.StartAmount; ++unitID)
                {
                    GameObject obj = GameObject.Instantiate(m_UnitPrefab[(int)unitDefinition.UnitType]) as GameObject;

                    PlayerColor playerColor = PlayerColor.White;
                    if (playerID == 1) playerColor = PlayerColor.Black;

                    VisualUnit visualUnit = obj.GetComponent<VisualUnit>();
                    visualUnit.SetPlayerColor(playerColor);
                    visualUnit.SetUnitDefinition(unitDefinition);
                    visualUnit.SetReserveTile(m_ReserveVisualTiles[playerID]);
                    visualUnit.SetParentTransform(gameObject.transform);
                    visualUnit.SetTile(null); //So it's positionned a the reserve tile

                    m_VisualUnits.Add(visualUnit);
                }
            }
        }
    }

    public void SetBoardState(BoardState boardState)
    {
        //Literally set the referebce, don't copy. Very dangerous, use with caution!!
        m_CurrentBoardState = boardState;
        LoadBoardState(m_CurrentBoardState);
        SaveBoardState();
    }

    public void SaveBoardState()
    {
        if (m_CurrentBoardState == null) return;

        for (int i = 0; i < m_VisualUnits.Count; ++i)
        {
            int tileID = m_VisualUnits[i].GetTile().ID;

            Tile currentTile = null;
            if (tileID != -1)
            {
                currentTile = CurrentBoardState.Tiles[tileID];
                //currentTile.SetUnit(CurrentBoardState.Units[i]);
            }

            CurrentBoardState.Units[i].SetTile(currentTile);
        }
    }

    public void LoadBoardState(BoardState boardState)
    {
        //Place all the units
        for (int i = 0; i < boardState.Units.Count; ++i)
        {
            Unit unit = boardState.Units[i];
            if (unit.GetTile() == null)
            {
                m_VisualUnits[i].SetTile(null);
            }
            else
            {
                VisualTile newTile = m_VisualTiles[unit.GetTile().ID];
                m_VisualUnits[i].SetTile(newTile);
            }
        }
    }

    public void EnableUnitSelection(PlayerColor playerColor, bool state)
    {
        for (int i = 0; i < m_VisualUnits.Count; ++i)
        {
            if (m_VisualUnits[i].GetPlayerColor() == playerColor)
            {
                m_VisualUnits[i].EnableDragging(state);
            }
        }
    }

    public void ShowUnits(PlayerColor playerColor, bool state)
    {
        for (int i = 0; i < m_VisualUnits.Count; ++i)
        {
            if (m_VisualUnits[i].GetPlayerColor() == playerColor)
            {
                m_VisualUnits[i].Show(state);
            }
        }
    }

    public void HighlightSetupZone(PlayerColor playerColor, bool enable)
    {
        //Determine begin & end tile
        int startTile = 0;
        if (playerColor == PlayerColor.White) startTile += 51;
        int endTile = startTile + 40;

        //Loop trough all of them and enable/disable them
        for (int i = startTile; i < endTile; ++i)
        {
            //Disallow chaining mountains
            if (VisualUnit.m_DraggedUnit != null && 
                VisualUnit.m_DraggedUnit.GetUnitDefinition().UnitType == UnitType.Mountain)
            {
                if (m_VisualTiles[i].HasNeightbourWithUnit(UnitType.Mountain, true, false))
                {
                    continue;
                }
            }

            m_VisualTiles[i].Highlight(enable);
        }
    }

    public void FlipBoard(bool enable)
    {
        //Flip the board
        float angle = 0.0f;
        if (enable) angle = 180.0f;

        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle));

        //Rotate the reserve tiles back
        foreach (VisualTile tile in m_ReserveVisualTiles)
        {
            tile.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle));
        }

        //Flip all the units again (otherwise they are upside down)
        FlipUnits(PlayerColor.White, enable, true);
        FlipUnits(PlayerColor.Black, enable, true);
    }

    public void FlipUnits(PlayerColor playerColor, bool enable, bool onlyOnBoard = false)
    {
        for (int i = 0; i < m_VisualUnits.Count; ++i)
        {
            if (m_VisualUnits[i].GetPlayerColor() == playerColor)
            {
                //If our tile is also our reserve tile we're not on the board
                if (onlyOnBoard && m_VisualUnits[i].GetTile() == m_VisualUnits[i].GetReserveTile())
                {
                    m_VisualUnits[i].Flip(false);
                }
                else
                {
                    m_VisualUnits[i].Flip(enable);
                }
            }
        }
    }
}