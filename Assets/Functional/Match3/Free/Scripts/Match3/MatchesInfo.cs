using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AN_Match3
{
    public class MatchesInfo
    {
        private readonly List<GameObject> matchedPieces;

        public MatchesInfo()
        {
            matchedPieces = new List<GameObject>();
        }

        /// <summary>
        ///     Returns distinct list of matched candy
        /// </summary>
        public IEnumerable<GameObject> MatchedCandy => matchedPieces.Distinct();

        private void AddObject(GameObject go)
        {
            if (!matchedPieces.Contains(go)) matchedPieces.Add(go);
        }

        public void AddObjectRange(IEnumerable<GameObject> gos)
        {
            foreach (var item in gos) AddObject(item);
        }
    }
}