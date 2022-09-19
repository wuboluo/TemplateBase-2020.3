using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class FixedExchangeController : MonoBehaviour
{
    [SerializeField] private SourceConfigEventChannelSO playSourceEvent;

    private Camera _mainCam;

    private FixedPiece _downPiece, _upPiece;
    private bool _pieceMoving;

    public SingleSourceConfigSO rightSwitchConfig, wrongSwitchConfig;

    public GameObject finger;
    public GameObject[] hummerPieces;

    public UnityEvent rightSwitch, wrongSwitch;

    private void Start()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (_pieceMoving) return;
        if (StateSwitcher.State != GameState.Gaming) return;

        if (Input.GetMouseButtonDown(0))
        {
            var ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                if (!hit.collider) return;
                _downPiece = hit.collider.GetComponent<FixedPiece>();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            var ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                if (!hit.collider || _downPiece == hit.collider.GetComponent<FixedPiece>()) return;

                _upPiece = hit.collider.GetComponent<FixedPiece>();

                if (IsNeighbors(_downPiece, _upPiece))
                {
                    var isRightSwitch = IsGuidePieces(_downPiece, _upPiece);

                    var switchCor = SwitchPieceCor(isRightSwitch);
                    StartCoroutine(switchCor);
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            var ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            if (hit.collider && hit.collider.CompareTag("Piece"))
            {
                hit.collider.GetComponent<Animator>().Play(hit.collider.name.Contains("Special") ? "Duang Special" : "Duang");
            }
        }
    }

    private IEnumerator SwitchPieceCor(bool successSwitch)
    {
        const float moveTime = 0.15f;
        _pieceMoving = true;

        var downPos = _downPiece.transform.position;
        var upPos = _upPiece.transform.position;

        _downPiece.transform.DOMove(upPos, moveTime);
        _upPiece.transform.DOMove(downPos, moveTime);

        yield return new WaitForSeconds(moveTime);

        if (successSwitch)
        {
            playSourceEvent.RaiseEvent(rightSwitchConfig);

            yield return new WaitForSeconds(moveTime);
            _pieceMoving = false;

            finger.SetActive(false);
            hummerPieces.ToList().ForEach(h => h.SetActive(false));
            rightSwitch.Invoke();
        }
        else
        {
            _downPiece.transform.DOMove(downPos, moveTime);
            _upPiece.transform.DOMove(upPos, moveTime);

            playSourceEvent.RaiseEvent(wrongSwitchConfig);

            yield return new WaitForSeconds(moveTime);
            _pieceMoving = false;
            wrongSwitch.Invoke();
        }
    }

    private static bool IsGuidePieces(FixedPiece p1, FixedPiece p2)
    {
        return p1.pieceType == PieceType.GuidePiece && p1.pieceType == p2.pieceType;
    }

    private static bool IsNeighbors(Component p1, Component p2)
    {
        if (p1 == null || p2 == null) return false;
        return Vector3.Distance(p1.transform.localPosition, p2.transform.localPosition) < 1f;
    }
}