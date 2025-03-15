using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystem<T>
{

    public event EventHandler<OnGridValueChangedEvent> GridValueChanged;
    public class OnGridValueChangedEvent : EventArgs
    {
        public int x;
        public int y;
    }
    private int width;
    private int height;
    private T[,] gridArray;
    private Vector3 origin;
    private float cellSize;
    public GridSystem(int width, int height, float cellSize, Vector3 origin, Func<GridSystem<T>, int, int, T> createDefaultObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;
        gridArray = new T[width, height];
        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                gridArray[i, j] = createDefaultObject(this, i, j);
            }
        }

        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.black, 100f);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.black, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetWidth()
    {
        return width;
    }

    public float GetCellSize()
    {
        return cellSize;
    }
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + origin;
    }

    public Vector2Int GetPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos - origin).x / cellSize);
        int y = Mathf.FloorToInt((worldPos - origin).y / cellSize);
        return new Vector2Int(x, y);
    }

    public void SetValue(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            if (GridValueChanged != null) GridValueChanged(this, new OnGridValueChangedEvent { x = x, y = y });
        }
    }

    public void TriggerGridChange(int x, int y)
    {
        if (GridValueChanged != null) GridValueChanged(this, new OnGridValueChangedEvent { x = x, y = y });
    }

    public void SetValue(Vector3 worldPos, T value)
    {
        Vector2Int position = GetPosition(worldPos);

        SetValue(position.x, position.y, value);
    }

    public T GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(T);
        }
    }

    public T GetValue(Vector3 worldPos)
    {
        Vector2Int position = GetPosition(worldPos);
        return GetValue(position.x, position.y);
    }
}