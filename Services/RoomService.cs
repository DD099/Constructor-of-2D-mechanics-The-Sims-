using System.Drawing;
using System.Globalization;
using Microsoft.Extensions.Options;
using SimsConstructor.Models;
using SimsConstructor.Models.Interfaces;
using SimsConstructor.Models.Items;
using SimsConstructor.Options;

namespace SimsConstructor.Services;

public sealed class RoomService
{
    private readonly List<RoomItem> _items = [];
    private readonly PlacementValidator _placement;
    private bool _initialized;

    public RoomService(PlacementValidator placement, IOptions<RoomSettings> roomOptions)
    {
        _placement = placement;
        var r = roomOptions.Value;
        WidthUnits = r.WidthMeters > 0f ? r.WidthMeters : 6f;
        HeightUnits = r.HeightMeters > 0f ? r.HeightMeters : 4f;
    }

    public float WidthUnits { get; }

    public float HeightUnits { get; }

    public IReadOnlyList<RoomItem> Items => _items;

    public string? StatusMessage { get; private set; }

    public void SetStatus(string? message) => StatusMessage = message;

    public void EnsureSampleRoom()
    {
        if (_initialized)
            return;

        _initialized = true;

        var bed = new FurnitureItem(
            "Bed",
            "Double bed",
            Color.SteelBlue,
            2f,
            1.6f,
            FurnitureCategory.Bed,
            "Wood",
            PlacementRule.WallRequired,
            899m);
        bed.PlaceAt(0.4f, 0.4f);

        var table = new FurnitureItem(
            "Dining table",
            "Seats four",
            Color.SaddleBrown,
            1.4f,
            0.9f,
            FurnitureCategory.Table,
            "Wood",
            PlacementRule.FloorOnly,
            449m);
        table.PlaceAt(3.2f, 1.1f);

        var tv = new ApplianceItem(
            "TV",
            "Wall-mounted panel",
            Color.DimGray,
            1.2f,
            0.15f,
            120m,
            false,
            false,
            699m);
        tv.PlaceAt(0.5f, 0.05f);

        var rug = new DecorationItem(
            "Rug",
            "Area rug",
            Color.IndianRed,
            2f,
            1.2f,
            DecorationStyle.Modern,
            false,
            129m);
        rug.PlaceAt(2.8f, 2.4f);

        _items.AddRange([bed, table, tv, rug]);
    }

    public void Select(RoomItem item)
    {
        foreach (var i in _items)
            i.Deselect();

        item.Select();
        StatusMessage = null;
    }

    public RoomItem? GetSelected() => _items.FirstOrDefault(i => i.IsSelected);

    public bool TryPlaceSelected(float x, float y)
    {
        var selected = GetSelected();
        if (selected is null)
        {
            StatusMessage = "Nothing selected.";
            return false;
        }

        if (!_placement.IsValidPosition(selected, x, y, WidthUnits, HeightUnits, _items))
        {
            StatusMessage = "Invalid position.";
            return false;
        }

        selected.PlaceAt(x, y);
        StatusMessage =
            $"{selected.Name}: {x.ToString(CultureInfo.InvariantCulture)}, {y.ToString(CultureInfo.InvariantCulture)}";
        return true;
    }

    public bool TryRotateSelected()
    {
        var selected = GetSelected();
        if (selected is null)
        {
            StatusMessage = "Nothing selected.";
            return false;
        }

        selected.Rotate90();

        if (!_placement.IsValidPosition(selected, selected.X, selected.Y, WidthUnits, HeightUnits, _items))
        {
            selected.Rotate90();
            selected.Rotate90();
            selected.Rotate90();
            StatusMessage = "Invalid rotation.";
            return false;
        }

        StatusMessage = $"{selected.Name}: {selected.Rotation} deg";
        return true;
    }

    public decimal GetTotalPrice()
    {
        decimal sum = 0;
        foreach (var item in _items)
        {
            if (item is IPricable p)
                sum += p.GetPrice();
        }

        return sum;
    }
}
