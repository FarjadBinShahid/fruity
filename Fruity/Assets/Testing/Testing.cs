using core.general.datamodels;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        MakeGrid(5, 4, 10);
    }


    #region Debugging and testing

    private void MakeGrid(int width, int height, float size)
    {
        Grid<bool> grid = new Grid<bool>(width, height, size, Vector3.one * 10, (grid, x,y) => new bool());
    }

    #endregion
}
