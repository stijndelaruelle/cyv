using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VisualBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject m_TilePrefab = null;

    [SerializeField]
    private GameObject m_ReserveTilePrefab = null;

    [SerializeField]
    private GameObject[] m_UnitPrefab = null;

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
        GenerateHexBoard(6);
        GenerateUnits();

        //Create empty board state
        BoardState boardState = new BoardState();
        boardState.GenerateDefaultState(6);

        m_CurrentBoardState = boardState;

        //LoadBoardState(boardState);
    }

    private void GenerateHexBoard(int boardSize)
    {
        if (m_VisualTiles == null) m_VisualTiles = new List<VisualTile>();
        m_VisualTiles.Clear();

        float width = 44;
        float height = 38;

        //Reserve tile
        GameObject obj = GameObject.Instantiate(m_ReserveTilePrefab) as GameObject;

        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.SetParent(transform.parent.GetComponent<RectTransform>());
        rectTransform.anchoredPosition = new Vector2(0.0f, -175.0f);
        rectTransform.sizeDelta = new Vector2(1.0f, 170.0f);
        rectTransform.localScale = new Vector2(1.0f, 1.0f);

        m_VisualTiles.Add(obj.GetComponent<VisualTile>());

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
                obj.GetComponent<VisualTile>().SetColor(m_Colors[color]);

                VisualTile newTile = obj.GetComponent<VisualTile>();
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

        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 110.0f);
        transform.localScale = new Vector3(0.75f, 0.75f, 1.0f);
    }

    private void GenerateUnits()
    {
        if (m_VisualUnits == null) m_VisualUnits = new List<VisualUnit>();
        m_VisualUnits.Clear();

        //6 Mountains, 1 king, 6 Rabble, 2 crossbows, 2 spears, 2 light horse, 2 catapults, 2 elephants, 2 heavy horse, 1 dragon = 26
        MountainUnitDefinition mountainUnitDefinition = new MountainUnitDefinition();
        KingUnitDefinition kingUnitDefinition = new KingUnitDefinition();

        int tempTileId = 0;

        for (int playerID = 0; playerID < 2; ++playerID)
        {
            ////Add mountains
            //for (int unitID = 0; unitID < mountainUnitDefinition.StartAmount; ++unitID)
            //{
            //    GameObject obj = GameObject.Instantiate(m_UnitPrefab[(int)mountainUnitDefinition.UnitType]) as GameObject;

            //    obj.transform.SetParent(m_VisualTiles[tempTileId].transform); //Parent this to the reserve slot
            //    obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            //    Color color = Color.white;
            //    if (playerID == 1) color = Color.black;

            //    VisualUnit visualUnit = obj.GetComponent<VisualUnit>();
            //    visualUnit.SetColor(color);
            //    m_VisualUnits.Add(visualUnit);

            //    ++tempTileId;
            //}

            //Add a king
            for (int unitID = 0; unitID < kingUnitDefinition.StartAmount; ++unitID)
            {
                GameObject obj = GameObject.Instantiate(m_UnitPrefab[(int)kingUnitDefinition.UnitType]) as GameObject;

                obj.transform.SetParent(m_VisualTiles[tempTileId].transform); //Parent this to the reserve slot
                obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                Color color = Color.white;
                if (playerID == 1) color = Color.black;

                VisualUnit visualUnit = obj.GetComponent<VisualUnit>();
                visualUnit.SetColor(color);
                m_VisualUnits.Add(visualUnit);

                ++tempTileId;
            }

            //Add ...
        }
    }

    public void AImove(int unitID, int fromID, int toID)
    {

    }
}