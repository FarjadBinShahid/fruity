using CodeMonkey.Utils;
using core.general.datamodels;
using fruity.gameplay.grid;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


namespace fruity.gameplay.buildingsystem
{
    public class GridBuildingsystem : MonoBehaviour
    {

        [Header("Grid Settings")]
        [Tooltip("Number of rows in grid")]
        [SerializeField] private int gridWidth;
        [Tooltip("Number of columns in grid")]
        [SerializeField] private int gridHeight;
        [Tooltip("Size of grid cell")]
        [SerializeField] private int cellSize;


        [Header("Placeable Objects")]
        [SerializeField] private List<PlaceableObjectSO> placeableObjectSOList;

        private PlaceableObjectSO placeableObjectSO;

        private Transform buildingParent;
        private Grid2D<GridObject> grid;
        private Direction dir = Direction.Down;

        private void Awake()
        {
            buildingParent = new GameObject("Buildings").transform;
            grid = new Grid2D<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid2D<GridObject> _grid, int _x, int _y) => new GridObject(_grid, _x, _y));
            placeableObjectSO = placeableObjectSOList[0];
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Build();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ChangePlaceableObjectDirection();
            }

        }

        private void Build()
        {
            int _x, _y;
            grid.GetXY(UtilsClass.GetMouseWorldPosition(), out _x, out _y);
            List<Vector2Int> gridPositionList = placeableObjectSO.GetGridPositionInList(new Vector2Int(_x, _y), dir);
            bool _canBuild = true;

            GridObject _gridObject;
            foreach (Vector2Int position in gridPositionList)
            {
                _gridObject = grid.GetGridObject(position.x, position.y);


                if (_gridObject == null)
                {
                    // cannot build here
                    _canBuild = false;
                    UtilsClass.CreateWorldTextPopup("Cannot build here!", UtilsClass.GetMouseWorldPosition());
                    break;
                }
                else if (!_gridObject.CanBuild())
                {
                    // cannot build here
                    _canBuild = false;
                    UtilsClass.CreateWorldTextPopup("Cannot build here!", UtilsClass.GetMouseWorldPosition());
                    break;
                }
            }

            if (_canBuild)
            {
                Vector2Int rotationOffset = placeableObjectSO.GetRotationOffset(dir);
                Vector3 placcedObjectWorlPosition = grid.GetWorldPosition(_x, _y) +
                    new Vector3(rotationOffset.x, rotationOffset.y, 0) * grid.GetCellSize();

                Transform _buildTransform = Instantiate(
                    placeableObjectSO.Prefab,
                    placcedObjectWorlPosition,
                    Quaternion.Euler(0, 0, placeableObjectSO.GetRotationAngle(dir)),
                    buildingParent);
                foreach (Vector2Int position in gridPositionList)
                {
                    grid.GetGridObject(position.x, position.y).SetTransform(_buildTransform);
                }
            }
            else
            {
                UtilsClass.CreateWorldTextPopup("Cannot build here!", UtilsClass.GetMouseWorldPosition());
            }
        }

        private void ChangePlaceableObjectDirection()
        {
            dir = PlaceableObjectSO.GetNextDirection(dir);
        }
    }
}