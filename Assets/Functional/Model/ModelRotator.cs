using UnityEngine;

public class ModelRotator : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    
    private float _dampSpeed;
    private float _mouseMoveDis;
    private float _xValue;
    
    private bool _onDrag;

    private void LateUpdate()
    {
        transform.Rotate(new Vector3(0, _xValue, 0) * RiSpeed(), Space.World);
        if (!Input.GetMouseButtonDown(0)) _onDrag = false;
    }

    private void OnMouseDown()
    {
        _xValue = 0f;
    }

    private void OnMouseDrag()
    {
        _onDrag = true;
        _xValue = -Input.GetAxis("Mouse X");
        _mouseMoveDis = Mathf.Sqrt(_xValue * _xValue);
        if (_mouseMoveDis == 0f) _mouseMoveDis = 1f;
    }

    private float RiSpeed()
    {
        if (_onDrag) _dampSpeed = speed;
        else _dampSpeed = 0;

        return _dampSpeed;
    }
}