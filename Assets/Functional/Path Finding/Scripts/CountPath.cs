using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CountPath : MonoBehaviour, IPathfinding
{
    public float moveSpeed;

    //Interval time between pathfinding
    [SerializeField] private float intervalTime = 1.0f;

    [SerializeField] private bool usePathSmoothing;

    [SerializeField] private bool showPathSmoothing;

    public Vector2 endPos;

    public float drawCubeSize = 1;

    private IEnumerator _currentPath;
    private Vector2 _endPosition;
    private Vector2[] _pathArray;
    private bool _readyToCountPath = true;
    private Transform _startPos;

    private Vector2 DrawCubeSize => new Vector2(drawCubeSize, drawCubeSize);


    // Draw path to gizmos
    public void OnDrawGizmos()
    {
        if (_pathArray == null) return;
        for (var i = 0; i < _pathArray.Length - 1; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(_pathArray[i], DrawCubeSize);
            Gizmos.DrawLine(_pathArray[i], _pathArray[i + 1]);
        }
    }

    public void OnPathFound(Vector2[] newPath)
    {
        if (_currentPath != null) StopCoroutine(_currentPath);
        _currentPath = MovePath(newPath);
        _pathArray = newPath;
        StartCoroutine(_currentPath);
    }

    public async void FindPath(Transform seeker, Vector2 endingPos)
    {
        if (!_readyToCountPath) return;

        if (seeker == null)
        {
            Debug.LogError("Missing seeker!", this);
            return;
        }

        _startPos = seeker;
        endPos = endingPos;
        //Basic raycast if can move directly to end target

        //bool cantSeeTarget = Physics2D.Linecast(_seeker.transform.position, _endPos, grid.unwalkableMask);
        //if (cantSeeTarget == false)
        //{
        //    Vector2[] newPath = new Vector2[1];
        //    newPath[0] = _endPos;
        //    OnPathFound(newPath);
        //    sw.Stop();
        //    print("Time took to find path: " + sw.ElapsedMilliseconds);
        //    StartCoroutine(PathCountDelay());
        //    return;
        //}

        if (endingPos == _endPosition) return;
        _endPosition = endingPos;
        await SearchPathRequest(this, seeker.position, _endPosition);
    }

    //This has not been tested
    public void StopMovement()
    {
        if (_currentPath != null) StopCoroutine(_currentPath);
    }

    private IEnumerator MovePath(IReadOnlyList<Vector2> pathArray)
    {
        if (pathArray == null) yield break;

        foreach (var t in pathArray)
            while ((Vector2) _startPos.transform.position != t)
            {
                var targetPos = t;
                Vector2 myPos = transform.position;
                targetPos.x -= myPos.x;
                targetPos.y -= myPos.y;
                // angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
                // transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                _startPos.transform.position = Vector2.MoveTowards(_startPos.transform.position, t, Time.deltaTime * moveSpeed);

                yield return null;
            }

        print("Auto move arrived ~~~~~~~");
    }

    private static Task<Vector3[]> SearchPathRequest(IPathfinding requester, Vector2 startPos, Vector2 endPos)
    {
        var taskCompletionSource = new TaskCompletionSource<Vector3[]>();

        var start = PathfindingGrid.Instance.NodeFromWorldPoint(startPos);
        var end = PathfindingGrid.Instance.ClosestNodeFromWorldPoint(endPos, start.GridAreaID);
        var newPath = AStar.FindPath(start, end);
        requester.OnPathFound(newPath);

        return taskCompletionSource.Task;
    }

    public IEnumerator PathCountDelay()
    {
        _readyToCountPath = false;
        var counter = Random.Range(intervalTime + 0.1f, intervalTime + 0.15f);
        yield return new WaitForSeconds(counter);
        _readyToCountPath = true;
    }
}