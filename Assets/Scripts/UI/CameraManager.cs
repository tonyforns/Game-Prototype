using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        HideActionCamera();
    }

    private void BaseAction_OnAnyActionCompleted(object sender, System.EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionStarted(object sender, System.EventArgs e)
    {
        switch(sender)
        {
            case ShootAction shootAction:

                SetActionCameraTransformOnShoot(shootAction);

                ShowActionCamera();
                break;
        }
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }

    private void SetActionCameraTransformOnShoot(ShootAction shootAction)
    {
        Unit shooterUnit = shootAction.GetUnit();
        Unit targetUnit = shootAction.GetTargetUnit();

        Vector3 cameraCharacterHeight = Vector3.up * 1.71f;

        Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

        float shouderOffsetAmount = 0.5f;
        Vector3 shoulderOffset = Quaternion.Euler(0f, 90f, 0f) * shootDir * shouderOffsetAmount;

        Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + shootDir * -1;

        actionCameraGameObject.transform.position = actionCameraPosition;
        actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
    }
}
