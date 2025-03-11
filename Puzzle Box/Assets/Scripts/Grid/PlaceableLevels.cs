using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Placeable")]
public class PlaceableLevels : ScriptableObject
{
    public GameObject prefab;
    public float width;
    public float height;

    public List<Vector2Int> GetGridPositionList(Vector2Int offset)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridPositionList.Add(new Vector2Int(i, j) + offset);
            }
        }
        return gridPositionList;
    }
}