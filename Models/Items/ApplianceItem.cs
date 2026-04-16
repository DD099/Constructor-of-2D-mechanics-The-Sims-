using System.Drawing;
using SimsConstructor.Models.Interfaces;

namespace SimsConstructor.Models.Items;

public class ApplianceItem : RoomItem, IPricable
{
    private decimal _price;

    public ApplianceItem(
        string name,
        string description,
        Color color,
        float width,
        float height,
        decimal powerConsumption,
        bool requiresWater,
        bool requiresVentilation,
        decimal price)
        : base(name, description, color, width, height)
    {
        PowerConsumption = powerConsumption >= 0
            ? powerConsumption
            : throw new ArgumentOutOfRangeException(nameof(powerConsumption));
        RequiresWater = requiresWater;
        RequiresVentilation = requiresVentilation;
        _price = price >= 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
    }

    public decimal PowerConsumption { get; }
    public bool RequiresWater { get; }
    public bool RequiresVentilation { get; }

    public decimal GetPrice() => _price;

    public override string ToString() => $"Appliance: {Name}";
}
