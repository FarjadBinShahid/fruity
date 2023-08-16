using core.general.datamodels;
using fruity.gameplay.buildingsystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fruity.gameplay.buildingsystem
{
    public class PlacedObject : MonoBehaviour
    {
        private PlaceableObjectSO placeableObjectSO;
        private Vector2Int origin;
        private Direction dir;



        public static PlacedObject CreatePlaceableObject(Vector3 worldPosition, Vector2Int origin, Direction dir, PlaceableObjectSO placeableObjectSO)
        {

            Transform placedObjectTransform = Instantiate(placeableObjectSO.Prefab, worldPosition, Quaternion.Euler(0, 0, placeableObjectSO.GetRotationAngle(dir)));

            PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
            placedObject.placeableObjectSO = placeableObjectSO;
            placedObject.dir = dir;

            return placedObject;

        }

        public List<Vector2Int> GetGridPositionList()
        {
            return placeableObjectSO.GetGridPositionInList(origin, dir);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

    }
}