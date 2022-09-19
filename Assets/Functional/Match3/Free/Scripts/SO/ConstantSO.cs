using UnityEngine;

namespace AN_Match3
{
    [CreateAssetMenu(fileName = "Game Data", menuName = "Yang/UC/Data")]
    public class ConstantSO : ScriptableObject
    {
        public int rows = 3;
        public int columns = 5;

        public float animationDuration = 0.15f;
        public float moveAnimationMinDuration = 0.15f;

        public float waitBeforePotentialMatchesCheck;
        public int minimumMatches = 3;

        public float pieceScale;
    }
}