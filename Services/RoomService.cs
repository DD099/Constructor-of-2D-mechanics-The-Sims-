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
    private readonly decimal? _budgetLimit;
    private bool _initialized;
    private readonly List<ItemTemplate> _catalog;

    public RoomService(PlacementValidator placement, IOptions<RoomSettings> roomOptions)
    {
        _placement = placement;
        var r = roomOptions.Value;
        WidthUnits = r.WidthMeters > 0f ? r.WidthMeters : 6f;
        HeightUnits = r.HeightMeters > 0f ? r.HeightMeters : 4f;
        _budgetLimit = r.BudgetLimit is decimal b && b > 0m ? b : null;
        _catalog = BuildCatalog();
    }

    public float WidthUnits { get; }

    public float HeightUnits { get; }

    public decimal? BudgetLimit => _budgetLimit;

    public bool IsOverBudget() =>
        _budgetLimit is not null && GetTotalPrice() > _budgetLimit.Value;

    public IReadOnlyList<RoomItem> Items => _items;

    public IReadOnlyList<ItemTemplate> Catalog => _catalog;

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

    public void ClearSelection()
    {
        foreach (var i in _items)
            i.Deselect();
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

    public bool TryAddFromCatalog(string templateId)
    {
        var template = _catalog.FirstOrDefault(t =>
            string.Equals(t.Id, templateId, StringComparison.OrdinalIgnoreCase));
        if (template is null)
        {
            StatusMessage = "Unknown template.";
            return false;
        }

        var item = template.Factory();
        item.RestoreFromSnapshot(0f, 0f, item.Width, item.Height, 0, placed: false);
        _items.Add(item);
        Select(item);
        StatusMessage = $"Added: {item.Name}";
        return true;
    }

    public bool RemoveSelected()
    {
        var selected = GetSelected();
        if (selected is null)
        {
            StatusMessage = "Nothing selected.";
            return false;
        }

        _items.Remove(selected);
        StatusMessage = $"Deleted: {selected.Name}";
        return true;
    }

    public string ExportLayoutJson() =>
        RoomLayoutSerializer.Serialize(_items, WidthUnits, HeightUnits);

    public bool TryImportLayoutJson(string json, out string? error)
    {
        if (!RoomLayoutSerializer.TryDeserialize(json, WidthUnits, HeightUnits, _placement, out var list, out error))
            return false;

        foreach (var i in _items)
            i.Deselect();

        _items.Clear();
        _items.AddRange(list!);
        _initialized = true;
        StatusMessage = "Imported.";
        error = null;
        return true;
    }

    private List<ItemTemplate> BuildCatalog()
    {
        return
        [
            new ItemTemplate(
                "bed-double",
                "Bed (double)",
                "Furniture",
                () => new FurnitureItem(
                    "Bed",
                    "Double bed",
                    Color.SteelBlue,
                    2f,
                    1.6f,
                    FurnitureCategory.Bed,
                    "Wood",
                    PlacementRule.WallRequired,
                    899m)),
            new ItemTemplate(
                "table-dining",
                "Dining table",
                "Furniture",
                () => new FurnitureItem(
                    "Dining table",
                    "Seats four",
                    Color.SaddleBrown,
                    1.4f,
                    0.9f,
                    FurnitureCategory.Table,
                    "Wood",
                    PlacementRule.FloorOnly,
                    449m)),
            new ItemTemplate(
                "sofa",
                "Sofa",
                "Furniture",
                () => new FurnitureItem(
                    "Sofa",
                    "Two-seat sofa",
                    Color.DarkOliveGreen,
                    1.8f,
                    0.9f,
                    FurnitureCategory.Seating,
                    "Fabric",
                    PlacementRule.FloorOnly,
                    799m)),
            new ItemTemplate(
                "tv",
                "TV",
                "Appliance",
                () => new ApplianceItem(
                    "TV",
                    "Wall-mounted panel",
                    Color.DimGray,
                    1.2f,
                    0.15f,
                    120m,
                    false,
                    false,
                    699m)),
            new ItemTemplate(
                "fridge",
                "Fridge",
                "Appliance",
                () => new ApplianceItem(
                    "Fridge",
                    "Kitchen fridge",
                    Color.LightSlateGray,
                    0.9f,
                    0.7f,
                    150m,
                    true,
                    false,
                    999m)),
            new ItemTemplate(
                "rug",
                "Rug",
                "Decoration",
                () => new DecorationItem(
                    "Rug",
                    "Area rug",
                    Color.IndianRed,
                    2f,
                    1.2f,
                    DecorationStyle.Modern,
                    false,
                    129m)),
            new ItemTemplate(
                "painting",
                "Painting",
                "Decoration",
                () => new DecorationItem(
                    "Painting",
                    "Wall decoration",
                    Color.Goldenrod,
                    1.2f,
                    0.8f,
                    DecorationStyle.Classic,
                    true,
                    159m))
        ];
    }

    public sealed record ItemTemplate(string Id, string Title, string Category, Func<RoomItem> Factory);
}
