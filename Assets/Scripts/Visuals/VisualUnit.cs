using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class VisualUnit : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //This class just visually represents the Unit & the player can interact with it.
    //It does in no way, shape or form hold any game data.

    //Events
    public VisualUnitDelegate OnPromote;

    public static VisualUnit m_DraggedUnit = null;

    //Our sprite, as it has to be higher up while draggin
    [SerializeField]
    private RectTransform m_SpriteTransform;

    //The big blob that is used for indicating the tile we're on
    [SerializeField]
    private RectTransform m_DragIndicator;

    private VisualTile m_Tile = null;
    private VisualTile m_ReserveTile = null; //The tile we need to go to when we die

    private UnitDefinition m_UnitDefinition = null; //Used for movement ranges etc.
    private PlayerColor m_PlayerColor = PlayerColor.White;

    private Transform m_ParentTransform;
    private bool m_IsDragged = false; //Used to distinguish between click movement & drag movement
    public bool IsDragged
    {
        get { return m_IsDragged; }
        set { m_IsDragged = value; }
    }

    private RectTransform m_CanvasTransform; //The canvas transform, used for drag & drop

    public void Awake()
    {
        m_CanvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
    }

    public void Select(VisualUnit unit)
    {
        if (m_DraggedUnit != null) { m_DraggedUnit.Deselect(); }
        m_DraggedUnit = unit;

        //Only enlarge units that we actually can pick up
        if (unit != null)
        {
            if (GameplayManager.Instance.CurrentPlayer == m_PlayerColor || MenuManager.Instance.IsInManual())
            {
                transform.localScale = new Vector3(3.0f, 3.0f, 1.0f);
            }
        }
    }

    public void Deselect()
    {
        m_DraggedUnit.ShowMovementRange(false);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        EnableDragging(true);

        //We placed the unit out of the valid fields
        if (transform.parent == m_ParentTransform)
        {
            SetTile(m_Tile);
        }
    }

    public void SetTile(VisualTile tile)
    {
        if (tile == null)
        {
            tile = m_ReserveTile;
        }
        else
        {
            ShowMovementRange(false);
        }

        m_Tile = tile;

        transform.SetParent(tile.GetUnitParent());
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //For some reason this resets
    }

    public void SetReserveTile(VisualTile reserveTile)
    {
        m_ReserveTile = reserveTile;
    }

    public VisualTile GetTile()
    {
        return m_Tile;
    }

    public VisualTile GetReserveTile()
    {
        return m_ReserveTile;
    }

    public void SetPlayerColor(PlayerColor playerColor)
    {
        m_PlayerColor = playerColor;

        //Color primColor = Color.white;
        //Color secColor = Color.black;
        //if (playerColor == PlayerColor.Black)
        //{
        //    primColor = Color.black;
        //    secColor = Color.white;
        //}

        //gameObject.GetComponent<Image>().color = primColor;

        ////if (m_UnitSprite != null)
        ////{
        ////    m_UnitSprite.color = secColor;
        ////}
    }

    public PlayerColor GetPlayerColor()
    {
        return m_PlayerColor;
    }

    public void SetUnitDefinition(UnitDefinition unitDefinition)
    {
        m_UnitDefinition = unitDefinition;
    }

    public UnitDefinition GetUnitDefinition()
    {
        return m_UnitDefinition;
    }

    public void SetParentTransform(Transform transform)
    {
        m_ParentTransform = transform;
    }

    private void ShowMovementRange(bool enable)
    {
        //Very special case, you can always go to your entire half of the board
        if (GameplayManager.Instance.GameState == GameState.Setup && !MenuManager.Instance.IsInManual())
        {
            GameplayManager.Instance.HighlightSetupZone(enable);
            return;
        }

        if (enable)
        {
            VisualTile.UnHighlightMovementHistory(true);
            VisualTile.UnHighlightMovementHistory(false);
        }
        else
        {
            VisualTile.HighlightMovementHistory(true);
            VisualTile.HighlightMovementHistory(false);
        }

        //Show movement range
        if (m_Tile != null && m_UnitDefinition != null)
        {
            for (int i = 0; i < BoardState.DIR_NUM; ++i)
            {
                m_Tile.HighlightNeighbour(i, enable, m_UnitDefinition.GetMoveCount(i, m_PlayerColor), m_UnitDefinition.CanJump, m_UnitDefinition.MustJump, m_PlayerColor);
            }
        }
    }

    public void EnableDragging(bool state)
    {
        gameObject.GetComponent<CanvasGroup>().blocksRaycasts = state;
    }

    public void Show(bool state)
    {
        gameObject.SetActive(state);
    }

    public void Flip(bool state)
    {
        float angle = 0.0f;
        if (state) angle = 180.0f;

        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, angle));
    }

    //----------------
    // INTERFACES
    //----------------

    //IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        //If we're allowed to promote this unit
        if (GameplayManager.Instance.GameState == GameState.Promotion && !MenuManager.Instance.IsInManual())
        {
            if (OnPromote != null) OnPromote(this);
            return;
        }

        //This means the user used the click & click method to place units
        if (GameplayManager.Instance.CurrentPlayer != m_PlayerColor && !MenuManager.Instance.IsInManual())
        {
            //If we are moving a character here, 
            if (m_DraggedUnit != null)
            {
                if (m_DraggedUnit.GetPlayerColor() == GameplayManager.Instance.CurrentPlayer || MenuManager.Instance.IsInManual())
                {
                    m_Tile.OnPointerDown(eventData);
                }
            }
            //If we're just checking the movement range
            else
            {
                Select(this);
                ShowMovementRange(true);
            }

            return;
        }

        OnBeginDrag(eventData);
        m_IsDragged = false;

        //Play a sound effect
        AudioManager.Instance.PlaySound(AudioManager.SoundType.GrabUnit);
    }

    //IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameplayManager.Instance.GameState == GameState.Promotion && !MenuManager.Instance.IsInManual())
            return;

        if (GameplayManager.Instance.CurrentPlayer != m_PlayerColor && !MenuManager.Instance.IsInManual())
            return;

        //Debug.Log("Begin Drag!");
        Select(this);

        //Loosen from field so we always render on top!
        transform.SetParent(m_ParentTransform);
        ShowMovementRange(true);

        EnableDragging(false);
        m_IsDragged = true;

        Vector3 newPoint = GetConvertedPosition(eventData);
        transform.position = newPoint;
 
        if (m_SpriteTransform != null)
            m_SpriteTransform.position = new Vector3(newPoint.x, newPoint.y + 1, newPoint.z);

        if (m_DragIndicator != null)
        {
            m_DragIndicator.gameObject.SetActive(true);
            m_DragIndicator.position = newPoint;
        }
    }

    //IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        if (GameplayManager.Instance.GameState == GameState.Promotion && !MenuManager.Instance.IsInManual())
            return;

        if (GameplayManager.Instance.CurrentPlayer != m_PlayerColor && !MenuManager.Instance.IsInManual())
            return;

        Vector3 newPoint = GetConvertedPosition(eventData);
        transform.position = newPoint;

        if (m_SpriteTransform != null)
            m_SpriteTransform.position = new Vector3(newPoint.x, newPoint.y + 1, newPoint.z);

        if (m_DragIndicator != null)
        {
            //Make the drag object snap to tiles
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                GameObject go = raycastResults[0].gameObject;

                //First is just an empty tile, second is when there is an unit on that tile (LAME but w/e)
                VisualTile visualTile = go.transform.parent.GetComponent<VisualTile>();
                if (visualTile == null) { visualTile = go.transform.parent.parent.parent.GetComponent<VisualTile>(); }

                if (visualTile != null && visualTile.IsHighlighted)
                {
                    m_DragIndicator.gameObject.SetActive(true);
                    m_DragIndicator.position = go.transform.position;
                }
                else
                {
                    m_DragIndicator.gameObject.SetActive(false);
                }
            }
        }
    }

    //IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        //We should always be able to deselect, otherwise showing the enemies movement range bugs out on the last used unit.
        Select(null);

        if (GameplayManager.Instance.GameState == GameState.Promotion && !MenuManager.Instance.IsInManual())
            return;

        Vector3 newPoint = GetConvertedPosition(eventData);
        transform.position = newPoint;

        if (m_SpriteTransform != null)
            m_SpriteTransform.position = newPoint;

        if (m_DragIndicator != null)
        {
            m_DragIndicator.gameObject.SetActive(false);
        }
            
    }

    private Vector3 GetConvertedPosition(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_CanvasTransform, Input.mousePosition, eventData.pressEventCamera, out localPoint);
        return m_CanvasTransform.TransformPoint(localPoint);
    }

}
