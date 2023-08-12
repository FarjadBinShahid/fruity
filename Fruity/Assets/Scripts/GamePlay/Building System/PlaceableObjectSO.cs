using core.general.datamodels;
using System.Collections.Generic;
using UnityEngine;

namespace fruity.gameplay.buildingsystem
{
    [CreateAssetMenu(fileName = "Placeable", menuName ="Scriptable Objects/PlaceableScriptableObject")]
    public class PlaceableObjectSO : ScriptableObject
    {
        public string NameString;
        public Transform Prefab;
        public Transform Visual;
        public int Width;
        public int Height;


        public List<Vector2Int> GetGridPositionInList(Vector2Int offset, Direction dir)
        {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();

            switch (dir)
            {
                default:
                case Direction.Down:
                case Direction.Up:
                    for(int x = 0; x<Width; x++)
                    {
                        for(int y = 0; y<Height; y++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
                case Direction.Left:
                case Direction.Right:
                    for (int x = 0; x < Height; x++)
                    {
                        for (int y = 0; y < Width; y++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
            }

            return gridPositionList;

        }

        public int GetRotationAngle(Direction dir)
        {
            switch (dir)
            {
                default:
                case Direction.Down: return 0;
                case Direction.Left: return 90;
                case Direction.Up: return 180;
                case Direction.Right: return 270;

            }
        }

        public Vector2Int GetRotationOffset(Direction dir)
        {
            switch (dir)
            {
                default:
                case Direction.Down: return new Vector2Int(0,0);
                case Direction.Left: return new Vector2Int(Height,0);
                case Direction.Up: return new Vector2Int(Width,Height);
                case Direction.Right: return new Vector2Int(0,Width);

            }
        }

        public static Direction GetNextDirection(Direction dir)
        {
            switch (dir)
            {
                default:
                case Direction.Down: return Direction.Left;
                case Direction.Left: return Direction.Up;
                case Direction.Up: return Direction.Right;
                case Direction.Right: return Direction.Down;

            }
        }

    }
}