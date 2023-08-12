using core.general.datamodels;
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


        private Grid<GridObject> grid;

        private void Awake()
        {
            grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid<GridObject> _grid, int _x, int _y) => new GridObject(_grid, _x, _y));
        }

    }
}