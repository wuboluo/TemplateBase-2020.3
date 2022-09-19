using UnityEngine;

namespace AN_Match3
{
    public class PieceInputController : MonoBehaviour
    {
        public FingerTimeListener fingerTimer;
        public ShapeManager shapeMgr;

        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void Update()
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                // 点击摁下：显示选中框 and 重置未操作时间
                if (Input.GetMouseButtonDown(0) && shapeMgr.state != Match3GameState.Animating)
                {
                    if (hit.collider.name != "Piece") return;
                    shapeMgr.m3Elements.clickRim.SetActive(true);
                    shapeMgr.m3Elements.clickRim.transform.position = hit.transform.position;

                    fingerTimer.ResetNoActionTime();
                }

                // 点击抬起：Q弹效果
                if (Input.GetMouseButtonUp(0))
                    if (hit.collider.name == "Piece")
                        hit.collider.transform.parent.GetComponent<Animator>().Play("Duang");
            }
        }
    }
}