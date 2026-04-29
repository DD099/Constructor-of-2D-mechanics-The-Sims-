namespace SimsConstructor.Models.Building;

public sealed record RoomArea(
    int Index,
    float X,
    float Y,
    float Width,
    float Height)
{
    public float Area => Width * Height;
}

