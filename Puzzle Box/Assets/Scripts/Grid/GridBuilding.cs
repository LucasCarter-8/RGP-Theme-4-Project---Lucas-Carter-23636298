using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GridBuilding : MonoBehaviour
{
    [SerializeField] public PlaceableLevels currentPlaceable;

    private PlayerInputManager inputManager;
    private PlayerController player;
    [SerializeField] private SFXEmitter emitter;
    [SerializeField] private GridSystem<GridObject> buildingGrid;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridCellSize;
    [SerializeField] private GameObject placementParticles;

    [SerializeField] private Tilemap backTiles;
    // Start is called before the first frame update
    private void Start()
    {
        backTiles = FindObjectOfType<Tilemap>();
        backTiles.SetTile(Vector3Int.zero, null);
        emitter = GetComponent<SFXEmitter>();
        buildingGrid = new GridSystem<GridObject>(gridWidth, gridHeight, gridCellSize, this.transform.position, (GridSystem<GridObject> grid, int x, int y) => new GridObject(x, y, grid));
        player = FindFirstObjectByType<PlayerController>();
        inputManager = player.GetComponent<PlayerInputManager>();
    }

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
        Vector2Int position = buildingGrid.GetPosition(GetMousePos());
        if (position.x >= 0 && position.y >= 0 && position.x < gridWidth && position.y < gridHeight)
        {
            if (inputManager.mouseLeftClick)
            {
                List<Vector2Int> gridPositions = currentPlaceable.GetGridPositionList(position);
                bool canPlace = true;
                foreach (Vector2Int gridPos in gridPositions)
                {
                    if (buildingGrid.GetValue(gridPos.x, gridPos.y) == null)
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
                    Vector3Int offset = new Vector3Int(-9, -1, 0);
                    PlacedObject placedObject = PlacedObject.Create(buildingGrid.GetWorldPosition(position.x, position.y), new Vector2Int(position.x, position.y), currentPlaceable);
                    foreach (Vector3Int gridPos in gridPositions)
                    {
                        buildingGrid.GetValue(gridPos.x, gridPos.y).SetPlacedObject(placedObject);

                        //Tilemap
                        for (int x = 0; x < gridWidth; x++)
                        {
                            for (int y = 0; y < gridHeight; y++)
                            {
                                
                                backTiles.SetTile(gridPos + offset, null);
                            }
                        }
                    }
                    //emitter.PlayOverlap(SoundEffectType.Place);
                    Vector2 particlePos = new Vector2(placedObject.transform.position.x + placedObject.placeableType.width / 2, placedObject.transform.position.y + placedObject.placeableType.height / 2);
                    //GameObject particles = Instantiate(placementParticles, particlePos, Quaternion.identity);

                }
                else
                {
                    Debug.Log("Cant place here");
                }


            }
            else if (inputManager.mouseRightClick)
            {
                GridObject gridObject = buildingGrid.GetValue(GetMousePos());
                PlacedObject placedObjectToDestroy = gridObject.GetPlacedObject();
                if (placedObjectToDestroy != null)
                {
                    List<Vector2Int> gridPositions = placedObjectToDestroy.GetGridPositionList();
                    foreach (Vector2Int gridPos in gridPositions)
                    {
                        buildingGrid.GetValue(gridPos.x, gridPos.y).RemovePlacedObject();

                    }
                    Destroy(placedObjectToDestroy.gameObject);
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
}