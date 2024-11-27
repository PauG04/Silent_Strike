using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraState { FRONT, MID, BACK, ACTION }

    [SerializeField] private CameraState currentState;
    private CameraState lastState;

    [Header("MidPoint")]
    [SerializeField] MidPointController midPointController;
    private Vector3 midPointPos;

    [Header("Lerp Speeds")]
    [SerializeField] private float widthLerpSpeed;
    [SerializeField] private float heightLerpSpeed;
    [SerializeField] private float depthLerpSpeed;
    [SerializeField] private float rotationLerpSpeed;

    [Header("Height References")]
    [SerializeField] private float heightOffsetBetweenZones;
    private float backHeight;
    private float midHeight;
    private float frontHeight;
    private float currentHeight;

    [Header("Depth References")]
    [SerializeField] private float depthOffsetFront;
    [SerializeField] private float depthOffsetMid;
    [SerializeField] private float depthOffsetBack;
    private float backDepth;
    private float midDepth;
    private float frontDepth;
    private float currentDepth;

    [Header("Rotation")]
    [SerializeField] private float XRotationFront;
    [SerializeField] private float XRotationMid;
    [SerializeField] private float XRotationBack;
    private Quaternion startRotation;
    private Quaternion endRotation;

    private Vector3 disCameraToHolder;
    [SerializeField] private CameraCollidersController _cameraCollidersController;

    // Start is called before the first frame update
    void Start()
    {
        disCameraToHolder = transform.position - Camera.main.transform.position;

        //Height
        frontHeight = transform.position.y - heightOffsetBetweenZones;
        midHeight = transform.position.y;
        backHeight = transform.position.y + heightOffsetBetweenZones;

        //Depth
        frontDepth = transform.position.z + depthOffsetFront;
        midDepth = transform.position.z - depthOffsetMid;
        backDepth = transform.position.z - depthOffsetBack;

        ChangeState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case CameraState.FRONT:
                CalculateNextPos();
                break;
            case CameraState.MID:
                CalculateNextPos();
                break;
            case CameraState.BACK:
                CalculateNextPos();
                break;
            case CameraState.ACTION:
                break;
            default:
                break;
        }
    }

    private void CalculateNextPos()
    {
        midPointPos = midPointController.transform.position;
        
        // Position Lerp
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, midPointPos.x,Time.deltaTime * widthLerpSpeed),
            Mathf.Lerp(transform.position.y, currentHeight, Time.deltaTime * heightLerpSpeed),
            Mathf.Lerp(transform.position.z, midPointPos.z + currentDepth, Time.deltaTime * depthLerpSpeed));

        // Lerp Rotation
        startRotation = transform.rotation;
        transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationLerpSpeed);
    }

    private void ZoomIn(float _zoomScalar)
    {
        ChangeState(CameraState.ACTION);

    }

    private void ZoomOut(float _zoomScalar)
    {
        ChangeState(CameraState.ACTION);

    }

    public void ZoomToGameObject(GameObject _target, float _zoomScalar)
    {
        ChangeState(CameraState.ACTION);

        transform.position = _target.transform.position;

        transform.position += disCameraToHolder.normalized * _zoomScalar; 
    }

    public void EndZoom()
    {

        _cameraCollidersController.CheckPlayerZone(PlayerManager.instance.transform.position);
    }

    public void ChangeState(CameraState _newState)
    {

        lastState = currentState;

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
                endRotation = Quaternion.Euler(XRotationFront, 0, 0);
                break;
            case CameraState.MID:
                currentHeight = midHeight; 
                currentDepth = midDepth;
                endRotation = Quaternion.Euler(XRotationMid, 0, 0);
                break;
            case CameraState.BACK:
                currentHeight = backHeight;
                currentDepth = backDepth;
                endRotation = Quaternion.Euler(XRotationBack, 0, 0);
                break;
            case CameraState.ACTION:
                break;
            default:
                break;
        }

        currentState = _newState;
    }
}
