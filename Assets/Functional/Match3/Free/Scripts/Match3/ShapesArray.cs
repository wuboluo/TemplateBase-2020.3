using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AN_Match3
{
    /// <summary>
    ///     Custom class to accomodate useful stuff for our shapes array
    ///     自定义类来容纳形状数组中有用的东西
    /// </summary>
    public class ShapesArray
    {
        private readonly GameObject[,] shapes =
            new GameObject[ShapeManager.GetInstance.constant.rows, ShapeManager.GetInstance.constant.columns];

        private GameObject backupG1;
        private GameObject backupG2;

        /// <summary>
        ///     Indexer
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public GameObject this[int row, int column]
        {
            get => shapes[row, column];
            set => shapes[row, column] = value;
        }

        /// <summary>
        ///     Swaps the position of two items, also keeping a backup
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        public void Swap(GameObject g1, GameObject g2)
        {
            //hold a backup in case no match is produced
            backupG1 = g1;
            backupG2 = g2;

            var g1Shape = g1.GetComponent<Shape>();
            var g2Shape = g2.GetComponent<Shape>();

            //get array indexes
            var g1Row = g1Shape.Row;
            var g1Column = g1Shape.Column;
            var g2Row = g2Shape.Row;
            var g2Column = g2Shape.Column;

            //swap them in the array
            var temp = shapes[g1Row, g1Column];
            shapes[g1Row, g1Column] = shapes[g2Row, g2Column];
            shapes[g2Row, g2Column] = temp;

            //swap their respective properties
            Shape.SwapColumnRow(g1Shape, g2Shape);
        }

        /// <summary>
        ///     Undoes the swap
        /// </summary>
        public void UndoSwap()
        {
            if (backupG1 == null || backupG2 == null)
                throw new Exception("Backup is null");

            Swap(backupG1, backupG2);
        }


        /// <summary>
        ///     Returns the matches found for a list of GameObjects
        ///     MatchesInfo class is not used as this method is called on subsequent collapses/checks,
        ///     not the one inflicted by user's drag
        /// </summary>
        /// <param name="gos"></param>
        /// <returns></returns>
        public IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos)
        {
            var matches = new List<GameObject>();
            foreach (var go in gos) matches.AddRange(GetMatches(go).MatchedCandy);
            return matches.Distinct();
        }

        /// <summary>
        ///     Returns the matches found for a single GameObject
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public MatchesInfo GetMatches(GameObject go)
        {
            var matchesInfo = new MatchesInfo();

            var horizontalMatches = GetMatchesHorizontally(go);


            matchesInfo.AddObjectRange(horizontalMatches);

            var verticalMatches = GetMatchesVertically(go);


            matchesInfo.AddObjectRange(verticalMatches);

            return matchesInfo;
        }

        /// <summary>
        ///     Searches horizontally for matches
        /// </summary>
        private IEnumerable<GameObject> GetMatchesHorizontally(GameObject go)
        {
            var matches = new List<GameObject> {go};
            var shape = go.GetComponent<Shape>();
            //check left
            if (shape.Column != 0)
                for (var column = shape.Column - 1; column >= 0; column--)
                    if (shapes[shape.Row, column].GetComponent<Shape>().IsSameType(shape))
                        matches.Add(shapes[shape.Row, column]);
                    else
                        break;

            //check right
            if (shape.Column != ShapeManager.GetInstance.constant.columns - 1)
                for (var column = shape.Column + 1; column < ShapeManager.GetInstance.constant.columns; column++)
                    if (shapes[shape.Row, column].GetComponent<Shape>().IsSameType(shape))
                        matches.Add(shapes[shape.Row, column]);
                    else
                        break;

            //we want more than three matches
            if (matches.Count < ShapeManager.GetInstance.constant.minimumMatches)
                matches.Clear();

            return matches.Distinct();
        }

        /// <summary>
        ///     Searches vertically for matches
        /// </summary>
        /// <param name="go"></param>
        private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
        {
            var matches = new List<GameObject> {go};
            var shape = go.GetComponent<Shape>();
            //check bottom
            if (shape.Row != 0)
                for (var row = shape.Row - 1; row >= 0; row--)
                    if (shapes[row, shape.Column] != null &&
                        shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
                        matches.Add(shapes[row, shape.Column]);
                    else
                        break;

            //check top
            if (shape.Row != ShapeManager.GetInstance.constant.rows - 1)
                for (var row = shape.Row + 1; row < ShapeManager.GetInstance.constant.rows; row++)
                    if (shapes[row, shape.Column] != null &&
                        shapes[row, shape.Column].GetComponent<Shape>().IsSameType(shape))
                        matches.Add(shapes[row, shape.Column]);
                    else
                        break;


            if (matches.Count < ShapeManager.GetInstance.constant.minimumMatches)
                matches.Clear();

            return matches.Distinct();
        }

        /// <summary>
        ///     Removes (sets as null) an item from the array
        /// </summary>
        /// <param name="item"></param>
        public void Remove(GameObject item)
        {
            if (shapes != null) shapes[item.GetComponent<Shape>().Row, item.GetComponent<Shape>().Column] = null;
        }

        /// <summary>
        ///     Collapses the array on the specific columns, after checking for empty items on them
        /// </summary>
        /// <param name="columns"></param>
        /// <returns>Info about the GameObjects that were moved</returns>
        public AlteredPieceInfo Collapse(IEnumerable<int> columns)
        {
            var collapseInfo = new AlteredPieceInfo();

            // search in every column
            foreach (var column in columns)
                //begin from bottom row
                for (var row = 0; row < ShapeManager.GetInstance.constant.rows - 1; row++)
                    //if you find a null item
                    if (shapes[row, column] == null)
                        //start searching for the first non-null
                        for (var row2 = row + 1; row2 < ShapeManager.GetInstance.constant.rows; row2++)
                            //if you find one, bring it down (i.e. replace it with the null you found)
                            if (shapes[row2, column] != null)
                            {
                                shapes[row, column] = shapes[row2, column];
                                shapes[row2, column] = null;

                                //calculate the biggest distance
                                if (row2 - row > collapseInfo.MaxDistance)
                                    collapseInfo.MaxDistance = row2 - row;

                                //assign new row and column (name does not change)
                                shapes[row, column].GetComponent<Shape>().Row = row;
                                shapes[row, column].GetComponent<Shape>().Column = column;

                                collapseInfo.AddPiece(shapes[row, column]);
                                break;
                            }

            return collapseInfo;
        }

        /// <summary>
        ///     Searches the specific column and returns info about null items
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public IEnumerable<ShapeInfo> GetEmptyItemsOnColumn(int column)
        {
            var emptyItems = new List<ShapeInfo>();
            for (var row = 0; row < ShapeManager.GetInstance.constant.rows; row++)
                if (shapes[row, column] == null)
                    emptyItems.Add(new ShapeInfo {Row = row, Column = column});
            return emptyItems;
        }
    }
}