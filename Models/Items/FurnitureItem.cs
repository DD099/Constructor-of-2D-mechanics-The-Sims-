using System.Drawing;
using SimsConstructor.Models;
using SimsConstructor.Models.Interfaces;

namespace SimsConstructor.Models.Items;

public class FurnitureItem : RoomItem, IPricable
{
    private decimal _price;

    public FurnitureItem(
        string name,
        string description,
        Color color,
        float width,
        float height,
        FurnitureCategory category,
        string material,
        PlacementRule placementRule,
        decimal price)
        : base(name, description, color, width, height)
    {
        Category = category;
        Material = material ?? string.Empty;
        PlacementRule = placementRule;
        _price = price >= 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
    }

    public FurnitureCategory Category { get; }
    public string Material { get; }
    public PlacementRule PlacementRule { get; }

    public decimal GetPrice() => _price;

    public bool CanPlaceNear(RoomItem? other)
    {
        if (other is null || ReferenceEquals(other, this))
            return false;

        return true;
    }

    public override string ToString() => $"Furniture: {Name} ({Category})";
}
