using UnityEngine;

[RequireComponent(typeof(CountPath))]
public class SeekerController : MonoBehaviour
{
    private CountPath _counter;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        _counter = GetComponent<CountPath>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                _counter.FindPath(transform, _camera.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }
}