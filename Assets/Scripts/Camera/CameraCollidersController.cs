using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollidersController : MonoBehaviour
{

    [Header("Position References")]
    [SerializeField] private GameObject frontCollider;
    [SerializeField] private GameObject backCollider;

    [Header("Camera")]
    [SerializeField] private CameraController cameraController;

    public void CheckPlayerZone(Vector3 _playerPosition)
    {
        if (_playerPosition.z > backCollider.transform.position.z)
        {
            cameraController.ChangeState(CameraController.CameraState.BACK);

        }
        else if (_playerPosition.z < backCollider.transform.position.z
              && _playerPosition.z > frontCollider.transform.position.z)
        {
            cameraController.ChangeState(CameraController.CameraState.MID);


        }
        else if (_playerPosition.z < frontCollider.transform.position.z)
        {
            cameraController.ChangeState(CameraController.CameraState.FRONT);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckPlayerZone(other.transform.position);
        }
    }
}
