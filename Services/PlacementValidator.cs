using System.Drawing;
using SimsConstructor.Models.Items;

namespace SimsConstructor.Services;

public sealed class PlacementValidator
{
    public bool IsValidPosition(
        RoomItem candidate,
        float x,
        float y,
        float roomWidth,
        float roomHeight,
        IEnumerable<RoomItem> allItems)
    {
        if (!FitsInsideRoom(candidate, x, y, roomWidth, roomHeight))
            return false;

        var proposed = BoundsAt(candidate, x, y);

        foreach (var other in allItems)
        {
            if (ReferenceEquals(other, candidate))
                continue;
            if (!other.IsPlaced)
                continue;

            if (proposed.IntersectsWith(other.GetRenderBounds()))
                return false;
        }

        return true;
    }

    private static RectangleF BoundsAt(RoomItem item, float x, float y) =>
        new(x, y, item.Width, item.Height);

    private static bool FitsInsideRoom(RoomItem item, float x, float y, float roomW, float roomH) =>
        x >= 0f
        && y >= 0f
        && x + item.Width <= roomW
        && y + item.Height <= roomH;
}
