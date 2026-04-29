namespace SimsConstructor.Models.Building;

public sealed record WallSegment(
    WallOrientation Orientation,
    float X1,
    float Y1,
    float X2,
    float Y2)
{
    public float MinX => MathF.Min(X1, X2);
    public float MaxX => MathF.Max(X1, X2);

    public float MinY => MathF.Min(Y1, Y2);
    public float MaxY => MathF.Max(Y1, Y2);

    public float Length =>
        Orientation == WallOrientation.Horizontal
            ? MaxX - MinX
            : MaxY - MinY;
}

