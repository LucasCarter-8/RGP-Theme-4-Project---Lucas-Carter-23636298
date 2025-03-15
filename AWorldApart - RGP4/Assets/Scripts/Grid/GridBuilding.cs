using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridBuilding : MonoBehaviour
{
    //Placeables
    [SerializeField] public PlaceableLevels currentPlaceable;
    [SerializeField] public PlaceableLevels startPlaceable;
    [SerializeField] public PlaceableLevels endPlaceable;

    private PlayerInputManager inputManager;
    private PlayerController player;
    [SerializeField] private SFXEmitter emitter;

    //Grid basics
    [SerializeField] private GridSystem<GridObject> buildingGrid;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridCellSize;

    [SerializeField] private GameObject placementParticles;

    //Tilemaps variables
    [SerializeField] private Tilemap backTiles;
    [SerializeField] private Tilemap blockedZoneTiles;
    [SerializeField] private TileBase backTileBase;
    [SerializeField] private Tilemap[] tilemaps;

    //Offset for tilemap
    private Vector3Int offset;

    //Level segments
    [SerializeField] private Transform startingPoint;
    [SerializeField] private string startTag;
    [SerializeField] private Transform endPoint;
    [SerializeField] private string endTag;
    [SerializeField] private SegmentPosition[] levelSegmentPositions;
    [SerializeField] private GameObject ghost;
    private GridGhost ghostComponent;
    [SerializeField] private bool isDragging = false;

    //UI prefabs
    [SerializeField] private GameObject popupText;
    private GameObject newPopup;

    // Start is called before the first frame update
    private void Start()
    {
        levelSegmentPositions = FindObjectsOfType<SegmentPosition>();
        tilemaps = FindObjectsOfType<Tilemap>();

        //Finding tilemaps
        for(int i = 0; i < tilemaps.Length; i++)
        {
            if (tilemaps[i].CompareTag("Background"))
            {
                backTiles = tilemaps[i];
            }
            else if (tilemaps[i].CompareTag("Blocked"))
            {
                blockedZoneTiles = tilemaps[i];
            }
        }
        emitter = GetComponent<SFXEmitter>();
        buildingGrid = new GridSystem<GridObject>(gridWidth, gridHeight, gridCellSize, this.transform.position, (GridSystem<GridObject> grid, int x, int y) => new GridObject(x, y, grid));
        player = FindFirstObjectByType<PlayerController>();
        inputManager = player.GetComponent<PlayerInputManager>();

        offset = new Vector3Int((int)this.transform.position.x, (int)this.transform.position.y, 0);

        //Creating ghost and making it invisible until the player has selected a new placeable
        ghost = Instantiate(ghost, Vector3.zero, Quaternion.identity);
        ghostComponent = ghost.GetComponent<GridGhost>();
        ghostComponent.ChangeGhost(null);

        //Set up Level (start, end and existing segments are created on the grid based off of transforms in the scene)

        //Start segment
        Vector2Int startPosition = buildingGrid.GetPosition(new Vector3(startingPoint.position.x, startingPoint.position.y));
        PlacedObject startObject = PlacedObject.Create(buildingGrid.GetWorldPosition(startPosition.x, startPosition.y), new Vector2Int(startPosition.x, startPosition.y), startPlaceable);
        startObject.gameObject.tag = startTag;
        List<Vector2Int> gridPositions = startPlaceable.GetGridPositionList(startPosition);
        foreach (Vector3Int gridPos in gridPositions)
        {
            buildingGrid.GetValue(gridPos.x, gridPos.y).SetPlacedObject(startObject);

            //Tilemap    
            backTiles.SetTile(gridPos + offset, null);

        }

        //End segment
        Vector2Int endPosition = buildingGrid.GetPosition(new Vector3(endPoint.position.x, endPoint.position.y));
        PlacedObject endObject = PlacedObject.Create(buildingGrid.GetWorldPosition(endPosition.x, endPosition.y), new Vector2Int(endPosition.x, endPosition.y), endPlaceable);
        //Find finish point in the object so that the sprite can change when the player completes the level
        foreach(SpriteRenderer child in endObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if(child.gameObject.CompareTag(endTag))
            {
                GameManager.Instance.finishPointRenderer = child;
                break;
            }
        }
        endObject.gameObject.tag = endTag;
        List<Vector2Int> endGridPositions = endPlaceable.GetGridPositionList(endPosition);
        foreach (Vector3Int gridPos in endGridPositions)
        {
            buildingGrid.GetValue(gridPos.x, gridPos.y).SetPlacedObject(endObject);

            //Tilemap    
            backTiles.SetTile(gridPos + offset, null);

        }

        //Other non-specific level segments
        for (int i = 0; i<levelSegmentPositions.Length; i++)
        {
            Vector2Int segmentPos = buildingGrid.GetPosition(new Vector3(levelSegmentPositions[i].transform.position.x, levelSegmentPositions[i].transform.position.y));
            PlacedObject segment = PlacedObject.Create(buildingGrid.GetWorldPosition(segmentPos.x, segmentPos.y), new Vector2Int(segmentPos.x, segmentPos.y), levelSegmentPositions[i].placeableType);
            List<Vector2Int> segmentGridPositions = levelSegmentPositions[i].placeableType.GetGridPositionList(segmentPos);
            foreach (Vector3Int gridPos in segmentGridPositions)
            {
                buildingGrid.GetValue(gridPos.x, gridPos.y).SetPlacedObject(segment);

                //Tilemap    
                backTiles.SetTile(gridPos + offset, null);

            }
        }
    }

    //The object that exists in each cell of the grid, each with their own object that resides within them
    public class GridObject
    {
        private int x;
        private int y;
        private PlacedObject placedObject;
        private GridSystem<GridObject> grid;

        public GridObject(int x, int y, GridSystem<GridObject> grid)
        {
            this.x = x;
            this.y = y;
            this.grid = grid;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridChange(x, y);
        }
        public void RemovePlacedObject()
        {
            placedObject = null;
            grid.TriggerGridChange(x, y);
        }

        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }
        public bool CanPlace()
        {
            if (placedObject == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    // Update is called once per frame
    private void Update()
    {
        //Getting mouse pos
        Vector2Int position = buildingGrid.GetPosition(GetMousePos());

        //Checking if mouse is within the grid
        if (position.x >= 0 && position.y >= 0 && position.x < gridWidth && position.y < gridHeight && !GameManager.Instance.finished)
        {          
            if (inputManager.mouseLeftClick && isDragging)
            {
                //Checking if the player can place anything down based on if it is within the grid
                List<Vector2Int> gridPositions = currentPlaceable.GetGridPositionList(position);
                bool canPlace = true;
                foreach (Vector3Int gridPos in gridPositions)
                {
                    if (buildingGrid.GetValue(gridPos.x, gridPos.y) == null || blockedZoneTiles.GetTile(gridPos + offset))
                    {
                        canPlace = false;
                        break;
                    }
                    else
                    {
                        if (!buildingGrid.GetValue(gridPos.x, gridPos.y).CanPlace())
                        {
                            canPlace = false;
                            break;
                        }
                    }

                }

                if (canPlace)
                {
                    //Creates the placed object as each designated position based on each size (which is determined by the type of placeable it is)
                    PlacedObject placedObject = PlacedObject.Create(buildingGrid.GetWorldPosition(position.x, position.y), new Vector2Int(position.x, position.y), currentPlaceable);
                    foreach (Vector3Int gridPos in gridPositions)
                    {
                        buildingGrid.GetValue(gridPos.x, gridPos.y).SetPlacedObject(placedObject);

                        //Tilemap    
                        backTiles.SetTile(gridPos + offset, null);

                    }
                    //emitter.PlayOverlap(SoundEffectType.Place);
                    Vector2 particlePos = new Vector2(placedObject.transform.position.x + placedObject.placeableType.width / 2, placedObject.transform.position.y + placedObject.placeableType.height / 2);
                    //GameObject particles = Instantiate(placementParticles, particlePos, Quaternion.identity);
                    ghostComponent.RemoveGhost();
                    isDragging = false;
                }
                else
                {
                    if(!newPopup)
                    CreateWorldText("Can't place here", GetMousePos());
                }


            }
            else if (inputManager.mouseRightClick && !isDragging)
            {
                GridObject gridObject = buildingGrid.GetValue(GetMousePos());
                PlacedObject placedObjectToDestroy = gridObject.GetPlacedObject();

                //Checks if the player can destroy a placeable (must not be null or other specific types)
                if (placedObjectToDestroy != null && !placedObjectToDestroy.gameObject.CompareTag(startTag) && !placedObjectToDestroy.gameObject.CompareTag(endTag))
                {
                    //Checks if the player is within a placeable, and if they are then the system will prevent it from being removed
                    bool noPlayerNearby = true;
                    List<Vector2Int> gridPositions = placedObjectToDestroy.GetGridPositionList();
                    foreach (Vector3Int gridPos in gridPositions)
                    {
                        if (Vector2.Distance(buildingGrid.GetPosition(player.transform.position), (Vector2Int)gridPos) <= 1f)
                        {   
                            noPlayerNearby = false;
                            break;
                        }
                    }
                    if (noPlayerNearby)
                    {
                        //Deleting every instance of the placeable in the grid at the right positions
                        currentPlaceable = placedObjectToDestroy.placeableType;
                        ghostComponent.ChangeGhost(currentPlaceable);
                        
                        foreach (Vector3Int gridPos in gridPositions)
                        {
                            buildingGrid.GetValue(gridPos.x, gridPos.y).RemovePlacedObject();

                            //Tilemap    
                            backTiles.SetTile(gridPos + offset, backTileBase);
                        }
                        Destroy(placedObjectToDestroy.gameObject);                       
                        isDragging = true;
                    }
                   
                }
            }
        }
    }

    private Vector2 GetMousePos()
    {
        Vector2 mousePos = new Vector2(Mouse.current.position.x.magnitude, Mouse.current.position.y.magnitude);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePos;
    }

    public Vector2 GetSnappedMousePos()
    {
        Vector2 mousePos = GetMousePos();
        return buildingGrid.GetPosition(mousePos) + (Vector2Int)offset;
    }

    private void CreateWorldText(string text, Vector2 position)
    {
        newPopup = Instantiate(popupText, position, Quaternion.identity);
        newPopup.GetComponentInChildren<TMP_Text>().text = text;
        newPopup.AddComponent<ProjectileMover>().Move(1f, 0.5f, Vector2.up);
    }
}