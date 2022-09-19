using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AN_Match3
{
    public class Shape : MonoBehaviour
    {
        public PieceType type;

        private GameObject _clearAnim;
        public int Column { get; set; }
        public int Row { get; set; }

        private void Start()
        {
            _clearAnim = transform.GetChild(0).gameObject;
        }

        public bool IsSameType(Shape otherShape)
        {
            if (!otherShape || !(otherShape != null))
                throw new ArgumentException("otherShape");

            return type == otherShape.type;
        }

        public void Assign(int row, int column)
        {
            Column = column;
            Row = row;
        }

        public static void SwapColumnRow(Shape a, Shape b)
        {
            var temp = a.Row;
            a.Row = b.Row;
            b.Row = temp;

            temp = a.Column;
            a.Column = b.Column;
            b.Column = temp;
        }

        public void OnClear()
        {
            Destroy(GetComponent<SpriteRenderer>());

            _clearAnim.SetActive(true);
            StartCoroutine(ClearCoroutine());
        }

        private IEnumerator ClearCoroutine()
        {
            yield return new WaitForSeconds(11f / 24f);

            Destroy(gameObject);
        }
    }

    public enum PieceType
    {
        Blue,
        Green,
        Purple,
        Red,
        Yellow
    }
}