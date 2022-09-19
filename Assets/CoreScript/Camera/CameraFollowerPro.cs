using UnityEngine;

public class CameraFollowerPro : MonoBehaviour
{
    [SerializeField] private FloatEventChannelSO onRatioUpdatedEvent;

    [SerializeField] private Transform follower;

    public float smoothing = 5f;

    private float _limitX, _limitY;

    private Camera _mainCam;

    private float _minX, _maxX;
    private float _minY, _maxY;

    private void Awake()
    {
        _mainCam = Camera.main;

        _limitX = _limitY = CameraAdapter.Instance.BgSize;
    }

    private void LateUpdate()
    {
        // Add a offset value if you need
        var followPos = follower.position + new Vector3(0, 0, 0);

        var pos = Vector3.Lerp(transform.position, followPos, smoothing * Time.deltaTime);
        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
        pos.y = Mathf.Clamp(pos.y, _minY, _maxY);
        pos.z = 0;

        transform.position = pos + new Vector3(0, 0, -10);
    }

    private void OnEnable()
    {
        onRatioUpdatedEvent.OnEventRaised += OnRatioUpdated;
    }

    private void OnDisable()
    {
        onRatioUpdatedEvent.OnEventRaised -= OnRatioUpdated;
    }

    private void OnRatioUpdated(float ratio)
    {
        UpdateLimitedArea();
    }

    private void UpdateLimitedArea()
    {
        var verticalExtent = _mainCam.orthographicSize;
        var horizontalExtent = verticalExtent * Screen.width / Screen.height;

        // 假设限制区域位于原点
        _minX = horizontalExtent - _limitX / 2;
        _maxX = _limitX / 2 - horizontalExtent;

        _minY = verticalExtent - _limitY / 2;
        _maxY = _limitY / 2 - verticalExtent;
    }
}