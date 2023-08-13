using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fruity.gameplay.grid
{
    public class Grid2D<TGridObject>
    {
        public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
        public class OnGridValueChangedEventArgs : EventArgs
        {
            public int x;
            public int y;
        }


        private int width;
        private int height;
        private float cellSize;

        private Vector3 originPosition;
        private TGridObject[,] gridArray;
        //private  List<List<TGridObject>> gridList = new List<List<TGridObject>>();

        private bool isDebuggingEnabled = true;

        public Grid2D(int width, int height, float cellSize, Vector3 originPosition, Func<Grid2D<TGridObject>, int, int, TGridObject> createdObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            this.gridArray = new TGridObject[width, height];

            /*for (int x = 0; x < width; x++)
            {
                gridList.Add(new List<TGridObject>());
                for (int y = 0; y < height; y++)
                {
                    gridList[x].Add(createdObject(this, x, y));
                }
            }*/


            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createdObject(this, x, y);
                }
            }

            if (isDebuggingEnabled)
            {
                TextMesh[,] debugTextArray = new TextMesh[width, height];
                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    for (int y = 0; y < gridArray.GetLength(1); y++)
                    {
                        debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null
                            , GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter);

                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.red);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.red);

                    }

                }
                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.red);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.red);

                OnGridValueChanged += (System.Object sender, OnGridValueChangedEventArgs eventArgs) =>
                {
                    debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
                };
            }
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * cellSize + originPosition;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        }

        public void SetGridObject(int x, int y, TGridObject value)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                gridArray[x, y] = value;
                OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
            }
            else
            {
                Debug.LogError($"invalid x : {x} > {width} or y : {y} > {height} for GetValue");
            }
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value)
        {
            int _x, _y;
            GetXY(worldPosition, out _x, out _y);
            SetGridObject(_x, _y, value);
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return gridArray[x, y];
            }
            else
            {
                Debug.LogError($"invalid x : {x} > {width} or y : {y} > {height} for SetValue");
                return default(TGridObject);
            }
        }


        public void TriggerGridObjectChanged(int x, int y)
        {
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });

        }

        public TGridObject GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return gridArray[x, y];
            }
            else
            {
                Debug.LogError($"invalid x : {x} or y : {y} for SetValue");
                return default(TGridObject);
            }
        }

        public TGridObject Getvalue(Vector3 worldPosition)
        {
            int _x, _y;
            GetXY(worldPosition, out _x, out _y);
            return GetValue(_x, _y);
        }

        public float GetCellSize()
        {
            return cellSize;
        }
    }
}