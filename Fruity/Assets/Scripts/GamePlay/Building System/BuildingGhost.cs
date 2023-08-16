using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace fruity.gameplay.buildingsystem
{
    public class BuildingGhost2D : MonoBehaviour
    {

        private Transform visual;
        private PlaceableObjectSO placedObjectTypeSO;

        private void Start()
        {
            RefreshVisual();
            GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
        }

        private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
        {
            RefreshVisual();
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
        }

        private void RefreshVisual()
        {
            if (visual != null)
            {
                Destroy(visual.gameObject);
                visual = null;
            }

            PlaceableObjectSO placedObjectTypeSO = GridBuildingSystem.Instance.GetPlacedObjectTypeSO();

            if (placedObjectTypeSO != null)
            {
                visual = Instantiate(placedObjectTypeSO.Visual, Vector3.zero, Quaternion.identity);
                visual.parent = transform;
                visual.localPosition = Vector3.zero;
                visual.localEulerAngles = Vector3.zero;
            }
        }

    }

}
