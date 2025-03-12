using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacedObject : MonoBehaviour
{
    public PlaceableLevels placeableType;
    [SerializeField] private Vector2Int origin;
    public static PlacedObject Create(Vector3 worldPos, Vector2Int origin, PlaceableLevels placeable)
    {
        Transform placedTransform = Instantiate(placeable.prefab.transform, worldPos, Quaternion.identity);
        PlacedObject placedObject = placedTransform.GetComponent<PlacedObject>();

        placedObject.origin = origin;
        placedObject.placeableType = placeable;
        return placedObject;
    }
    public List<Vector2Int> GetGridPositionList()
    {
        return placeableType.GetGridPositionList(origin);
    }
}