using CodeMonkey.Utils;
using core.architecture;
using core.general.datamodels;
using fruity.gameplay.grid;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


namespace fruity.gameplay.buildingsystem
{
    public class GridBuildingSystem : Singleton<GridBuildingSystem>
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


        public event EventHandler OnSelectedChanged;
        public event EventHandler OnObjectPlaced;


        protected override void Awake()
        {
            base.Awake();
            buildingParent = new GameObject("Buildings").transform;
            grid = new Grid2D<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid2D<GridObject> _grid, int _x, int _y) => new GridObject(_grid, _x, _y));
            placeableObjectSO = placeableObjectSOList[0];
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                BuildPlaceableObject();
            }else if(Input.GetMouseButtonDown(1))
            {
                DemolishPlacedObject();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ChangePlaceableObjectDirection();
            }

            if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                placeableObjectSO = placeableObjectSOList[0];
                RefreshSelectedObjectType();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                placeableObjectSO = placeableObjectSOList[1];
                RefreshSelectedObjectType();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                placeableObjectSO = placeableObjectSOList[2];
                RefreshSelectedObjectType();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                placeableObjectSO = placeableObjectSOList[3];
                RefreshSelectedObjectType();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                placeableObjectSO = placeableObjectSOList[4];
                RefreshSelectedObjectType();

            }
        }



        private void BuildPlaceableObject()
        {
            int _x, _y;
            grid.GetXY(UtilsClass.GetMouseWorldPosition(), out _x, out _y);
            List<Vector2Int> _gridPositionList = placeableObjectSO.GetGridPositionInList(new Vector2Int(_x, _y), dir);
            bool _canBuild = true;

            GridObject _gridObject;
            foreach (Vector2Int position in _gridPositionList)
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
                Vector2 placcedObjectWorlPosition = grid.GetWorldPosition(_x, _y) +
                    new Vector2(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

                PlacedObject _placedObject = PlacedObject.CreatePlaceableObject(placcedObjectWorlPosition, new Vector2Int(_x,_y), dir, placeableObjectSO);

                foreach (Vector2Int position in _gridPositionList)
                {
                    grid.GetGridObject(position.x, position.y).SetPlacedObject(_placedObject);
                }
                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                UtilsClass.CreateWorldTextPopup("Cannot build here!", UtilsClass.GetMouseWorldPosition());
            }
        }
        private void DemolishPlacedObject()
        {
            GridObject _gridObject = grid.GetGridObject(UtilsClass.GetMouseWorldPosition());
            PlacedObject _placedObject = _gridObject.GetPlacedObject();

            if(_placedObject != null)
            {
                _placedObject.DestroySelf();
                List<Vector2Int> _gridPositionList = _placedObject.GetGridPositionList();
                foreach (Vector2Int position in _gridPositionList)
                {
                    grid.GetGridObject(position.x, position.y).ClearPlacedObject();
                }
            }

            
        }

        private void DeselectObjectType()
        {
            placeableObjectSO = null; 
            RefreshSelectedObjectType();
        }

        private void RefreshSelectedObjectType()
        {
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }


        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            grid.GetXY(worldPosition, out int x, out int z);
            return new Vector2Int(x, z);
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            grid.GetXY(mousePosition, out int x, out int y);

            if (placeableObjectSO != null)
            {
                Vector2Int rotationOffset = placeableObjectSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector2(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
                return placedObjectWorldPosition;
            }
            else
            {
                return mousePosition;
            }
        }

        public Quaternion GetPlacedObjectRotation()
        {
            if (placeableObjectSO != null)
            {
                return Quaternion.Euler(0,0 , placeableObjectSO.GetRotationAngle(dir));
            }
            else
            {
                return Quaternion.identity;
            }
        }

        public PlaceableObjectSO GetPlacedObjectTypeSO()
        {
            return placeableObjectSO;
        }

        private void ChangePlaceableObjectDirection()
        {
            dir = PlaceableObjectSO.GetNextDirection(dir);
        }
    }
}