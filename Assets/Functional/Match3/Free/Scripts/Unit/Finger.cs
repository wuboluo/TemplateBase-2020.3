using UnityEngine;

namespace AN_Match3
{
    public class Finger : MonoBehaviour
    {
        private const float SmoothTime = 0.3f;
        private Vector3 _velocity;

        private Vector3 _fingerFirstPos;
        private Vector3 _fingerMoveTo;

        private bool _fingerGo;
        private bool _fingerLoop;

        private void Update()
        {
            if (!_fingerLoop) return;

            transform.position = _fingerGo
                ? Vector3.SmoothDamp(transform.position, _fingerMoveTo, ref _velocity, SmoothTime)
                : Vector3.SmoothDamp(transform.position, _fingerFirstPos, ref _velocity, SmoothTime);

            if (Vector3.Distance(transform.position, _fingerMoveTo) < 0.1f) _fingerGo = false;
            if (Vector3.Distance(transform.position, _fingerFirstPos) < 0.1f) _fingerGo = true;
        }

        public void Appear(Vector3 firstPos, Vector3 moveToPos)
        {
            if (gameObject.activeSelf) return;

            gameObject.SetActive(true);

            // 设置手指位置为 可拖动的片上
            transform.position = firstPos;

            // 记录手指位置，用来往返移动
            _fingerFirstPos = transform.position;
            _fingerMoveTo = moveToPos;

            _fingerGo = true;
            _fingerLoop = true;
        }

        public void Disappear()
        {
            gameObject.SetActive(false);
            _fingerLoop = false;

            _velocity = Vector3.zero;
        }
    }
}