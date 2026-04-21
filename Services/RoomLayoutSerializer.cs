using System.Drawing;
using System.Text.Json;
using SimsConstructor.Models;
using SimsConstructor.Models.Interfaces;
using SimsConstructor.Models.Items;

namespace SimsConstructor.Services;

public static class RoomLayoutSerializer
{
    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string Serialize(IReadOnlyList<RoomItem> items, float roomWidth, float roomHeight)
    {
        var doc = new LayoutDocument
        {
            Version = 1,
            RoomWidth = roomWidth,
            RoomHeight = roomHeight,
            Items = items.Select(ToRecord).ToList()
        };

        return JsonSerializer.Serialize(doc, WriteOptions);
    }

    public static bool TryDeserialize(
        string json,
        float roomWidth,
        float roomHeight,
        PlacementValidator placement,
        out List<RoomItem>? items,
        out string? error)
    {
        items = null;
        error = null;

        LayoutDocument? doc;
        try
        {
            doc = JsonSerializer.Deserialize<LayoutDocument>(json, ReadOptions);
        }
        catch (JsonException ex)
        {
            error = ex.Message;
            return false;
        }

        if (doc?.Items is null || doc.Items.Count == 0)
        {
            error = "Empty layout.";
            return false;
        }

        var list = new List<RoomItem>();
        foreach (var r in doc.Items)
        {
            var item = CreateItem(r);
            if (item is null)
            {
                error = "Unknown item kind.";
                return false;
            }

            item.RestoreFromSnapshot(r.X, r.Y, r.Width, r.Height, r.Rotation, r.Placed);
            list.Add(item);
        }

        foreach (var a in list)
        {
            if (!placement.IsValidPosition(a, a.X, a.Y, roomWidth, roomHeight, list))
            {
                error = "Layout does not fit the room or items overlap.";
                return false;
            }
        }

        items = list;
        return true;
    }

    private static LayoutItemRecord ToRecord(RoomItem item)
    {
        var price = item is IPricable p ? p.GetPrice() : 0m;
        var rec = new LayoutItemRecord
        {
            Name = item.Name,
            Description = item.Description,
            ColorArgb = item.Color.ToArgb(),
            X = item.X,
            Y = item.Y,
            Width = item.Width,
            Height = item.Height,
            Rotation = item.Rotation,
            Placed = item.IsPlaced,
            Price = price
        };

        switch (item)
        {
            case FurnitureItem f:
                rec.Kind = "furniture";
                rec.Material = f.Material;
                rec.FurnitureCategory = f.Category.ToString();
                rec.PlacementRule = f.PlacementRule.ToString();
                return rec;
            case ApplianceItem a:
                rec.Kind = "appliance";
                rec.PowerConsumption = a.PowerConsumption;
                rec.RequiresWater = a.RequiresWater;
                rec.RequiresVentilation = a.RequiresVentilation;
                return rec;
            case DecorationItem d:
                rec.Kind = "decoration";
                rec.DecorationStyle = d.Style.ToString();
                rec.IsWallMountable = d.IsWallMountable;
                return rec;
            default:
                throw new InvalidOperationException(item.GetType().Name);
        }
    }

    private static RoomItem? CreateItem(LayoutItemRecord r)
    {
        var color = Color.FromArgb(r.ColorArgb);
        return r.Kind.ToLowerInvariant() switch
        {
            "furniture" => new FurnitureItem(
                r.Name,
                r.Description,
                color,
                r.Width,
                r.Height,
                Enum.Parse<FurnitureCategory>(r.FurnitureCategory ?? nameof(FurnitureCategory.Table), true),
                r.Material ?? string.Empty,
                Enum.Parse<PlacementRule>(r.PlacementRule ?? nameof(PlacementRule.FloorOnly), true),
                r.Price),
            "appliance" => new ApplianceItem(
                r.Name,
                r.Description,
                color,
                r.Width,
                r.Height,
                r.PowerConsumption ?? 0m,
                r.RequiresWater ?? false,
                r.RequiresVentilation ?? false,
                r.Price),
            "decoration" => new DecorationItem(
                r.Name,
                r.Description,
                color,
                r.Width,
                r.Height,
                Enum.Parse<DecorationStyle>(r.DecorationStyle ?? nameof(DecorationStyle.Modern), true),
                r.IsWallMountable ?? false,
                r.Price),
            _ => null
        };
    }

    private sealed class LayoutDocument
    {
        public int Version { get; set; }

        public float RoomWidth { get; set; }

        public float RoomHeight { get; set; }

        public List<LayoutItemRecord> Items { get; set; } = [];
    }

    private sealed class LayoutItemRecord
    {
        public string Kind { get; set; } = "";

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public int ColorArgb { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public int Rotation { get; set; }

        public bool Placed { get; set; }

        public decimal Price { get; set; }

        public string? Material { get; set; }

        public string? FurnitureCategory { get; set; }

        public string? PlacementRule { get; set; }

        public decimal? PowerConsumption { get; set; }

        public bool? RequiresWater { get; set; }

        public bool? RequiresVentilation { get; set; }

        public string? DecorationStyle { get; set; }

        public bool? IsWallMountable { get; set; }
    }
}
