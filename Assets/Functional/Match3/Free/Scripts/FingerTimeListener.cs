using UnityEngine;

namespace AN_Match3
{
    public class FingerTimeListener : MonoBehaviour
    {
        public Finger finger;
        private ShapeManager _shapeMgr;

        public float waitTime = 5;
        private float _noActionTime;

        public bool TheFingerIsActive => finger.gameObject.activeSelf;

        private void Start()
        {
            _shapeMgr = ShapeManager.GetInstance;
        }

        private void Update()
        {
            if (_shapeMgr.state == Match3GameState.Animating || _shapeMgr.NoMove || TheFingerIsActive) return;

            _noActionTime += Time.deltaTime;

            if (!(_noActionTime > waitTime)) return;

            ResetNoActionTime();
            Match3GameManager.GetInstance.AppearFingerGuide();
        }

        public void ResetNoActionTime()
        {
            _noActionTime = 0;
        }
    }
}