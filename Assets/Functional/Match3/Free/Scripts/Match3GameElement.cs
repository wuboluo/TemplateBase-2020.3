using UnityEngine;

namespace AN_Match3
{
    public class Match3GameElement : MonoBehaviour
    {
        public Combo combo;
        public GameObject clickRim;

        public SingleSourceConfigSO rightConfig, wrongConfig;
        public GameObject[] piecePrefabs;
    }
}