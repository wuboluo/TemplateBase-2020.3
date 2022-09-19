using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AN_Match3
{
    public class ShapeManager : MonoBehaviour
    {
        public static ShapeManager GetInstance;
        
        [SerializeField] private IntEventChannelSO afterTimesSuccesses;
        [SerializeField] private SourceConfigEventChannelSO playSourceEvent;

        public ConstantSO constant;

        public Match3GameElement m3Elements;
        public Match3GameManager m3Manager;
        
        public Match3GameState state = Match3GameState.None;

        /////////////////////////////////////////////
        private IEnumerator _checkPotentialMatchesCor;

        private Camera _mainCam;
        private ShapesArray _shapes;

        private Vector3 _firstMatchP;
        private Vector3 _middleMatchP;

        private IEnumerable<GameObject> _potentialMatches;
        private Vector2[] _spawnPositions;

        private GameObject _hitGo;
        private int _moveTimes;

        /////////////////////////////////////////////
        private float PieceScale => constant.pieceScale;
        private Vector2 UnderLeftPiecePos { get; set; }

        public bool NoMove => _moveTimes == 0;
        public Vector3 PieceGuidePos { get; private set; }

        public Vector3 FingerMoveTo
        {
            get
            {
                var moveTo = Vector3.zero;
                if (Mathf.Abs(PieceGuidePos.x - _firstMatchP.x) > 0.01f &&
                    Mathf.Abs(PieceGuidePos.y - _firstMatchP.y) > 0.01f)
                {
                    const string hintLog = "提示：横竖都不在一条线上";

                    if (Mathf.Abs(_firstMatchP.x - _middleMatchP.x) < 0.01f)
                    {
                        print($"{hintLog}，左右划动");
                        moveTo = new Vector3(_firstMatchP.x, PieceGuidePos.y);
                    }
                    else if (Mathf.Abs(_firstMatchP.y - _middleMatchP.y) < 0.01f)
                    {
                        print($"{hintLog}，上下划动");
                        moveTo = new Vector3(PieceGuidePos.x, _firstMatchP.y);
                    }
                }
                else
                {
                    const string hintLog = "提示：同行或同列";
                    if (Mathf.Abs(_firstMatchP.y - _middleMatchP.y) < 0.01f)
                    {
                        if (PieceGuidePos.x > _firstMatchP.x)
                        {
                            print($"{hintLog}，同行，左划");
                            moveTo = new Vector3(PieceGuidePos.x - PieceScale, PieceGuidePos.y);
                        }
                        else
                        {
                            print($"{hintLog}，同行，右划");
                            moveTo = new Vector3(PieceGuidePos.x + PieceScale, PieceGuidePos.y);
                        }
                    }
                    else if (Mathf.Abs(_firstMatchP.x - _middleMatchP.x) < 0.01f)
                    {
                        if (PieceGuidePos.y > _firstMatchP.y)
                        {
                            print($"{hintLog}，同列，下划");
                            moveTo = new Vector3(PieceGuidePos.x, PieceGuidePos.y - PieceScale);
                        }
                        else
                        {
                            print($"{hintLog}，同列，上划");
                            moveTo = new Vector3(PieceGuidePos.x, PieceGuidePos.y + PieceScale);
                        }
                    }
                }

                return moveTo;
            }
        }

        /////////////////////////////////////////////

        private void Awake()
        {
            GetInstance = this;
        }

        private void Start()
        {
            _mainCam = Camera.main;

            InitAndSpawnPieces();
        }

        private void Update()
        {
            switch (state)
            {
                case Match3GameState.None:
                {
                    // user has clicked or touched
                    if (Input.GetMouseButtonDown(0))
                    {
                        // get the hit position
                        var hit = Physics2D.Raycast(_mainCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        if (!hit.collider) return;

                        _hitGo = hit.collider.gameObject;

                        state = Match3GameState.SelectionStarted;
                    }

                    break;
                }
                case Match3GameState.SelectionStarted:
                {
                    //user dragged
                    if (Input.GetMouseButtonUp(0))
                    {
                        var hit = Physics2D.Raycast(_mainCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        // we have a hit
                        if (hit.collider && _hitGo != hit.collider.gameObject)
                        {
                            // user did a hit, no need to show him hints 
                            StopCheckForPotentialMatches();

                            // if the two shapes are diagonally aligned (different row and column), just return
                            var hitShape = hit.collider.gameObject.GetComponent<Shape>();

                            if (!Utilities.AreVerticalOrHorizontalNeighbors(_hitGo.GetComponent<Shape>(), hitShape))
                            {
                                state = Match3GameState.None;
                            }
                            else
                            {
                                state = Match3GameState.Animating;
                                StartCoroutine(FindMatchesAndCollapse(hit));
                            }
                        }
                    }

                    break;
                }
            }
        }

        public void SetBoardLayout(int row, int column, Vector2 underLeftPiecePos)
        {
            constant.rows = row;
            constant.columns = column;

            UnderLeftPiecePos = underLeftPiecePos;
        }

        public void InitAndSpawnPieces()
        {
            m3Manager.ResetFingerNoActionTime();
            m3Elements.combo.singleComboAmount = 0;

            if (_shapes != null) DestroyAllPiece();

            _shapes = new ShapesArray();
            _spawnPositions = new Vector2[constant.columns];

            for (var row = 0; row < constant.rows; row++)
            for (var column = 0; column < constant.columns; column++)
            {
                var newCandy = GetRandomPiece();

                //check if two previous horizontal are of the same type
                while (column >= 2
                       && _shapes[row, column - 1].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>())
                       && _shapes[row, column - 2].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>()))
                    newCandy = GetRandomPiece();

                //check if two previous vertical are of the same type
                while (row >= 2
                       && _shapes[row - 1, column].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>())
                       && _shapes[row - 2, column].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>()))
                    newCandy = GetRandomPiece();

                InstantiateAndPlaceNewPiece(row, column, newCandy);
            }

            SetupSpawnPositions();

            StartCheckForPotentialMatches();
        }

        private void InstantiateAndPlaceNewPiece(int row, int column, GameObject newCandy)
        {
            var go = Instantiate(newCandy,
                UnderLeftPiecePos + new Vector2(column * PieceScale, row * PieceScale), Quaternion.identity, transform);

            go.transform.localScale = new Vector3(PieceScale, PieceScale, 1);

            //assign the specific properties
            go.GetComponent<Shape>().Assign(row, column);
            _shapes[row, column] = go;
        }

        //     为新形状创建生成位置（将从 "天花板" 弹出）
        private void SetupSpawnPositions()
        {
            for (var column = 0; column < constant.columns; column++)
                // 结尾处 -1 是不想让生成点高于棋盘，所以以最上层位置生成
                _spawnPositions[column] =
                    UnderLeftPiecePos + new Vector2(column * PieceScale, (constant.rows - 1) * PieceScale);
        }

        private void DestroyAllPiece()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var shape = transform.GetChild(i).gameObject;
                Destroy(shape);
            }
        }

        //     查找匹配项并折叠
        private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit2)
        {
            m3Elements.clickRim.SetActive(false);

            //get the second item that was part of the swipe
            var hitGo2 = hit2.collider.gameObject;
            _shapes.Swap(_hitGo, hitGo2);

            //move the swapped ones
            _hitGo.transform.DOMove(hitGo2.transform.position, constant.animationDuration);
            hitGo2.transform.DOMove(_hitGo.transform.position, constant.animationDuration);
            yield return new WaitForSeconds(constant.animationDuration);

            //get the matches via the helper methods
            var hitGoMatchesInfo = _shapes.GetMatches(_hitGo);
            var hitGo2MatchesInfo = _shapes.GetMatches(hitGo2);

            var totalMatches = hitGoMatchesInfo.MatchedCandy.Union(hitGo2MatchesInfo.MatchedCandy).Distinct();

            // 如果用户的交换没有创建至少 3 匹配，撤消他们的交换
            if (totalMatches.Count() < constant.minimumMatches)
            {
                _hitGo.transform.DOMove(hitGo2.transform.position, constant.animationDuration);
                hitGo2.transform.DOMove(_hitGo.transform.position, constant.animationDuration);
                yield return new WaitForSeconds(constant.animationDuration);

                _shapes.UndoSwap();

                // 实例一个错误的音效
                playSourceEvent.RaiseEvent(m3Elements.wrongConfig);
            }

            var timesRun = 0;
            m3Elements.combo.singleComboAmount = timesRun;

            while (totalMatches.Count() >= constant.minimumMatches)
            {
                print("成功消除一组");
                m3Manager.ResetFingerNoActionTime();
                m3Manager.DisappearFingerGuide();

                // 实例一个正确的音效
                playSourceEvent.RaiseEvent(m3Elements.rightConfig);

                // todo: 点击引导棋子，关闭遮罩

                foreach (var item in totalMatches)
                {
                    _shapes.Remove(item);
                    RemoveFromScene(item);
                }

                //get the columns that we had a collapse
                var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();

                //the order the 2 methods below get called is important!!!
                //collapse the ones gone
                var collapsedCandyInfo = _shapes.Collapse(columns);
                //create new ones
                var newCandyInfo = CreateNewPieceInSpecificColumns(columns);

                var maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);

                MoveAndAnimate(newCandyInfo.AlteredPiece, maxDistance);
                MoveAndAnimate(collapsedCandyInfo.AlteredPiece, maxDistance);

                //will wait for both of the above animations
                yield return new WaitForSeconds(constant.moveAnimationMinDuration * maxDistance);

                //search if there are matches with the new/collapsed items
                totalMatches = _shapes.GetMatches(collapsedCandyInfo.AlteredPiece)
                    .Union(_shapes.GetMatches(newCandyInfo.AlteredPiece)).Distinct();

                timesRun++;
                m3Elements.combo.singleComboAmount = timesRun;
            }

            // 刷新 Combo
            UpdateCombo();

            // 成功消除
            OnSuccessEliminate();

            state = Match3GameState.None;

            // 完成一步操作，不论成功
            print("--- Finish one step ---");
            StartCheckForPotentialMatches();
        }

        //     在缺少棋子的列中生成新棋子
        private AlteredPieceInfo CreateNewPieceInSpecificColumns(IEnumerable<int> columnsWithMissingPiece)
        {
            var newPieceInfo = new AlteredPieceInfo();

            //find how many null values the column has
            foreach (var column in columnsWithMissingPiece)
            {
                var emptyItems = _shapes.GetEmptyItemsOnColumn(column);
                foreach (var item in emptyItems)
                {
                    var go = GetRandomPiece();
                    var newCandy = Instantiate(go, _spawnPositions[column], Quaternion.identity, transform);
                    newCandy.transform.localScale = new Vector3(PieceScale, PieceScale, 1);

                    newCandy.GetComponent<Shape>().Assign(item.Row, item.Column);

                    if (constant.rows - item.Row > newPieceInfo.MaxDistance)
                        newPieceInfo.MaxDistance = constant.rows - item.Row;

                    _shapes[item.Row, item.Column] = newCandy;
                    newPieceInfo.AddPiece(newCandy);
                }
            }

            return newPieceInfo;
        }

        /// <summary>
        ///     将游戏对象动画到它们的新位置
        /// </summary>
        /// <param name="movedGameObjects"></param>
        /// <param name="distance"></param>
        private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance)
        {
            foreach (var item in movedGameObjects)
                item.transform.DOMove(UnderLeftPiecePos + new Vector2(item.GetComponent<Shape>().Column * PieceScale, item.GetComponent<Shape>().Row * PieceScale),
                    constant.moveAnimationMinDuration * distance);
        }

        /// <summary>
        ///     成功消除一组，并随机生成一组消除特效
        /// </summary>
        /// <param name="item"></param>
        private void RemoveFromScene(GameObject item)
        {
            // Destroy(item);
            item.GetComponent<Shape>().OnClear();
        }

        private GameObject GetRandomPiece()
        {
            return m3Elements.piecePrefabs[Random.Range(0, m3Elements.piecePrefabs.Length)];
        }

        /// <summary>
        ///     开始检查潜在匹配
        /// </summary>
        private void StartCheckForPotentialMatches()
        {
            print("开始检查潜在匹配");

            StopCheckForPotentialMatches();

            // 获得参考以稍后停止
            _checkPotentialMatchesCor = CheckPotentialMatches();
            StartCoroutine(_checkPotentialMatchesCor);
        }

        /// <summary>
        ///     停止检查潜在匹配
        /// </summary>
        private void StopCheckForPotentialMatches()
        {
            // print("停止检查潜在匹配");
            if (_checkPotentialMatchesCor != null)
                StopCoroutine(_checkPotentialMatchesCor);
        }

        /// <summary>
        ///     查找潜在匹配项
        /// </summary>
        private IEnumerator CheckPotentialMatches()
        {
            yield return new WaitForSeconds(constant.waitBeforePotentialMatchesCheck);
            _potentialMatches = Utilities.GetPotentialMatches(_shapes);
            if (_potentialMatches != null)
            {
                while (true)
                {
                    print("当前存在潜在匹配项");

                    // 待匹配的两个的位置
                    _firstMatchP = _potentialMatches.ToList()[0].transform.position;
                    _middleMatchP = _potentialMatches.ToList()[1].transform.position;

                    // 该移动的片的位置
                    var count = _potentialMatches.ToList().Count;
                    var pos = _potentialMatches.ToList()[count - 1].transform.position;
                    PieceGuidePos = pos;

                    // todo: ！！！
                    // if (mapAdapter.isTurn)
                    // {
                    //     print("横竖变化时，更新手指位置");
                    //     mapAdapter.isTurn = false;
                    //
                    //     m3Manager.AppearFingerGuide();
                    // }

                    if (_moveTimes == 0)
                    {
                        print("以棋盘引导开始时，更新手指位置");
                        m3Manager.AppearFingerGuide();
                    }

                    break;
                }
            }
            else
            {
                print("死棋");
                InitAndSpawnPieces();
            }
        }

        private void UpdateCombo()
        {
            m3Elements.combo.UpdateCombo();
        }

        /// <summary>
        ///     成功消除时，增加成功次数
        /// </summary>
        private void OnSuccessEliminate()
        {
            _moveTimes += m3Elements.combo.singleComboAmount;
            if (m3Elements.combo.singleComboAmount > 1)
                _moveTimes -= m3Elements.combo.singleComboAmount - 1;

            afterTimesSuccesses.RaiseEvent(_moveTimes);
        }
    }

    public enum Match3GameState
    {
        None,
        SelectionStarted,
        Animating
    }
}