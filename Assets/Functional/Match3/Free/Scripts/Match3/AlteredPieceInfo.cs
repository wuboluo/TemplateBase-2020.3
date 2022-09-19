using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AN_Match3
{
    // 改变棋子信息
    public class AlteredPieceInfo
    {
        public AlteredPieceInfo()
        {
            NewPieces = new List<GameObject>();
        }

        private List<GameObject> NewPieces { get; }
        public int MaxDistance { get; set; }

        /// <summary>
        ///     Returns distinct list of altered candy
        ///     返回不同的改变的棋子列表
        /// </summary>
        public IEnumerable<GameObject> AlteredPiece => NewPieces.Distinct();

        public void AddPiece(GameObject go)
        {
            if (!NewPieces.Contains(go)) NewPieces.Add(go);
        }
    }
}