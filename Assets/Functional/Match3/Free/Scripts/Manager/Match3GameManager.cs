using System;
using UnityEngine;

namespace AN_Match3
{
    public class Match3GameManager : MonoBehaviour
    {
        public static Match3GameManager GetInstance;
        
        public ShapeManager shapeManager;
        public FingerTimeListener fingerTimer;

        private void Awake()
        {
            GetInstance = this;
        }

        public void AppearFingerGuide()
        {
            fingerTimer.finger.Appear(shapeManager.PieceGuidePos, shapeManager.FingerMoveTo);
        }

        public void DisappearFingerGuide()
        {
            fingerTimer.finger.Disappear();
        }

        public void ResetFingerNoActionTime()
        {
            fingerTimer.ResetNoActionTime();
        }
    }
}