
using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace core.general.datamodels
{
    public class DataModels : MonoBehaviour
    {

    }

    #region Downloaded Images

    public enum DownloadedImagestypes
    {

    }


    #endregion

    #region UI
    #region Popups

    public enum PopupsType
    {
        NoInternet,
        InputBlocker,
        Loading,
        GeneralError
    }

    #endregion


    #region Views

    public enum ViewsType
    {
        MainView
    }

    #endregion
    #endregion

    #region General Enums

    public enum TooltipMessageType
    {

    }

    public enum PopupMetaKeys
    {

    }

    public enum Direction
    {
        Down,Left,Up,Right
    }

    #endregion

    #region MetaData

    [Serializable]
    public class MetaData
    {
        public GameMetaConstants GameMetaConstants;
    }

    [Serializable]
    public class GameMetaConstants
    {
        public UnitConstants UnitConstants;

        public Dictionary<PopupMetaKeys, PopupMessagesConstants> PopupMessages = new Dictionary<PopupMetaKeys, PopupMessagesConstants>();
        public Dictionary<TooltipMessageType, string> TooltipMessages = new Dictionary<TooltipMessageType, string>();
    }

    [Serializable]
    public class UnitConstants
    {

    }

    #region PopupMeta
    [Serializable]
    public class PopupMessagesConstants
    {
        public string Title;
        public string Message;

        public PopupMessagesConstants()
        {
            Title = "Title";
            Message = "Message";
        }

        public PopupMessagesConstants(PopupMessagesConstants value)
        {
            Title = value.Title;
            Message = value.Message;
        }

        public PopupMessagesConstants(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }



    #endregion
    #region Meta Helper Classes

    [Serializable]
    public class SerializedVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializedVector3()
        {

        }

        public SerializedVector3(SerializedVector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

        public SerializedVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public SerializedVector3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }


        public Vector3 GetVector3() => new Vector3(x, y, z);

    }

    [Serializable]
    public class SerializedColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public Color Color { get; set; }

        public SerializedColor()
        {
        }

        public SerializedColor(SerializedColor color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;

            Color = new Color(r, g, b, a);
        }

        public SerializedColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            Color = new Color(r, g, b, a);
        }

        public SerializedColor(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
            Color = new Color(r, g, b, a);
        }

        public Color GetColor() => new Color(r, g, b, a);
    }
    #endregion
    #endregion


    #region Grid

    public class Grid<TGridObject>
    {
        public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
        public class OnGridValueChangedEventArgs : EventArgs
        {
            public int x;
            public int y;
        }


        public int Width;
        public int Height;
        private float cellSize;

        private Vector3 originPosition;
        private TGridObject[,] gridArray;

        private bool isDebuggingEnabled = true;

        public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createdObject)
        {
            Width = width;
            Height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            this.gridArray = new TGridObject[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createdObject(this, x,y);
                }
            }

            if (isDebuggingEnabled)
            {
                TextMesh[,] debugTextArray = new TextMesh[width, height];
                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    for (int y = 0; y < gridArray.GetLength(1); y++)
                    {
                        debugTextArray[x,y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null
                            , GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter);

                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.red, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.red, 100f);

                    }

                }
                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.red, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.red, 100f);

                OnGridValueChanged += (Object sender, OnGridValueChangedEventArgs eventArgs) =>
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
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                gridArray[x, y] = value;
                OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
            }
            else
            {
                Debug.LogError($"invalid x : {x} or y : {y} for GetValue");
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
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                return gridArray[x, y];
            }
            else
            {
                Debug.LogError($"invalid x : {x} or y : {y} for SetValue");
                return default(TGridObject);
            }
        }


        public void TriggerGridObjectChanged(int x, int y)
        {
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });

        }

        public TGridObject GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
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


    public class GridObject
    {
        private int x;
        private int y;
        private Grid<GridObject> grid;
        private Transform transform;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
            grid.TriggerGridObjectChanged(x,y);
        }

        public void ClearTransform()
        {
            transform = null;
        }

        public bool CanBuild()
        {
            return transform == null;
        }

        public override string ToString()
        {
            return $" {x} , {y}";
        }

        
    }



    #endregion

}