using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraState { FRONT, MID, BACK, ACTION }

    [SerializeField] private CameraState currentState;
    [SerializeField] MidPointController midPointController;

    private float heightReference;

    private Vector3 newPos;


    // Start is called before the first frame update
    void Start()
    {
        heightReference = transform.position.y;
        newPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 5.0f);
    }

    public void ChangeState(CameraState _newState)
    {
        switch (currentState)
        {
            case CameraState.FRONT:
                break;
            case CameraState.MID:
                break;
            case CameraState.BACK:

                break;
            case CameraState.ACTION:
                break;
            default:
                break;
        }

        switch (_newState)
        {
            case CameraState.FRONT:
                newPos = new Vector3(transform.position.x, heightReference - 1.0f, transform.position.z);
                break;
            case CameraState.MID:
                newPos = new Vector3(transform.position.x, heightReference, transform.position.z);
                break;
            case CameraState.BACK:
                newPos = new Vector3(transform.position.x, heightReference + 1.0f, transform.position.z);
                break;
            case CameraState.ACTION:
                break;
            default:
                break;
        }

        currentState = _newState;
    }
}
