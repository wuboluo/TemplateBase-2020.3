using UnityEngine;

public static class Direction
{
    private const float Tan20 = 0.3639702343f;
    private const float Cot20 = 2.7474774195f;

    public static DirectionType ReturnDirection(Vector3 fromPos, Vector3 toPos)
    {
        // 相同位置
        if (toPos.x == fromPos.x && toPos.y == fromPos.y) return DirectionType.Null;

        // 正方向
        if (toPos.x == fromPos.x)
        {
            if (toPos.y > fromPos.y) return DirectionType.Top;
            if (toPos.y < fromPos.y) return DirectionType.Bottom;
        }

        if (toPos.y == fromPos.y)
        {
            if (toPos.x > fromPos.x) return DirectionType.Right;
            if (toPos.x < fromPos.x) return DirectionType.Left;
        }

        // 组合方向
        if (toPos.x > fromPos.x)
        {
            if (toPos.y > fromPos.y) return DirectionType.RightTop;
            if (toPos.y < fromPos.y) return DirectionType.RightBottom;
        }
        else
        {
            if (toPos.y > fromPos.y) return DirectionType.LeftTop;
            if (toPos.y < fromPos.y) return DirectionType.LeftBottom;
        }

        // if (fromPos.x >= toPos.x && fromPos.y >= toPos.y)
        // {
        //     double endAngle = (fromPos.y - toPos.y) / (fromPos.x - toPos.x);
        //
        //     if (Tan20 < endAngle && endAngle < Cot20) return DirectionType.LeftBottom;
        //     return endAngle < Cot20 ? DirectionType.Left : DirectionType.Bottom;
        // }
        //
        // if (fromPos.x <= toPos.x && fromPos.y >= toPos.y)
        // {
        //     double endAngle = Mathf.Abs(fromPos.y - toPos.y) / Mathf.Abs(fromPos.x - toPos.x);
        //
        //     if (Tan20 < endAngle && endAngle < Cot20) return DirectionType.RightBottom;
        //     return endAngle < Cot20 ? DirectionType.Right : DirectionType.Bottom;
        // }
        //
        // if (fromPos.x <= toPos.x && fromPos.y <= toPos.y)
        // {
        //     double endAngle = Mathf.Abs(fromPos.y - toPos.y) / Mathf.Abs(fromPos.x - toPos.x);
        //
        //     if (Tan20 < endAngle && endAngle < Cot20) return DirectionType.RightTop;
        //     return endAngle < Cot20 ? DirectionType.Right : DirectionType.Top;
        // }
        //
        // if (fromPos.x >= toPos.x && fromPos.y <= toPos.y)
        // {
        //     double endAngle = Mathf.Abs(fromPos.y - toPos.y) / (fromPos.x - toPos.x);
        //
        //     if (Tan20 < endAngle && endAngle < Cot20) return DirectionType.LeftTop;
        //     return endAngle < Cot20 ? DirectionType.Left : DirectionType.Top;
        // }

        return DirectionType.Null;
    }
}

public enum DirectionType
{
    Null,

    Top,
    Bottom,
    Right,
    Left,

    RightTop,
    RightBottom,
    LeftTop,
    LeftBottom
}