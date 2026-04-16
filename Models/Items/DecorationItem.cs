using System.Drawing;
using SimsConstructor.Models;
using SimsConstructor.Models.Interfaces;

namespace SimsConstructor.Models.Items;

public class DecorationItem : RoomItem, IPricable
{
    private decimal _price;

    public DecorationItem(
        string name,
        string description,
        Color color,
        float width,
        float height,
        DecorationStyle style,
        bool isWallMountable,
        decimal price)
        : base(name, description, color, width, height)
    {
        Style = style;
        IsWallMountable = isWallMountable;
        _price = price >= 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
    }

    public DecorationStyle Style { get; }
    public bool IsWallMountable { get; }

    public decimal GetPrice() => _price;

    public override string ToString() => $"Decoration: {Name} ({Style})";
}
