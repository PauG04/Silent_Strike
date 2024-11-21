using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraState { FRONT, MID, BACK, ACTION }

    [SerializeField] private CameraState currentState;

    [Header("MidPoint")]
    [SerializeField] MidPointController midPointController;
    private Vector3 midPointPos;

    private Vector3 newPos;

    [Header("Lerp Speeds")]
    [SerializeField] private float widthLerpSpeed;
    [SerializeField] private float heightLerpSpeed;
    [SerializeField] private float depthLerpSpeed;

    [Header("Height References")]
    [SerializeField] private float heightOffsetBetweenZones;
    private float backHeight;
    private float midHeight;
    private float frontHeight;
    private float currentHeight;

    [Header("Depth References")]
    [SerializeField] private float depthOffsetBetweenZones;
    private float backDepth;
    private float midDepth;
    private float frontDepth;
    private float currentDepth;



    // Start is called before the first frame update
    void Start()
    {
        newPos = transform.position;

        //Height
        frontHeight = transform.position.y - heightOffsetBetweenZones;
        midHeight = transform.position.y;
        backHeight = transform.position.y + heightOffsetBetweenZones;

        //Depth
        frontDepth = transform.position.z + depthOffsetBetweenZones;
        midDepth = transform.position.z;
        backDepth = transform.position.z - depthOffsetBetweenZones;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateNextPos();
    }

    private void CalculateNextPos()
    {
        midPointPos = midPointController.transform.position;
        

        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, midPointPos.x,Time.deltaTime * widthLerpSpeed),
            Mathf.Lerp(transform.position.y, currentHeight, Time.deltaTime * heightLerpSpeed),
            Mathf.Lerp(transform.position.z, currentDepth, Time.deltaTime * depthLerpSpeed)); 
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
                currentHeight = frontHeight;
                currentDepth = frontDepth;
                break;
            case CameraState.MID:
                currentHeight = midHeight; 
                currentDepth = midDepth;
                break;
            case CameraState.BACK:
                currentHeight = backHeight;
                currentDepth = midDepth;
                break;
            case CameraState.ACTION:
                break;
            default:
                break;
        }

        currentState = _newState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(midPointPos, 1);
    }
}
