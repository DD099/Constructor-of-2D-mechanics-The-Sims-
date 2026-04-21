using System.Drawing;
using SimsConstructor.Models.Core;
using SimsConstructor.Models.Interfaces;

namespace SimsConstructor.Models.Items;

public abstract class RoomItem : GameObject, IPlaceable, IRotatable, ISelectable
{
    private bool _placed;
    private float _x;
    private float _y;

    protected RoomItem(string name, string description, Color color, float width, float height)
        : base(name, description, color)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
    }

    public float X
    {
        get => _x;
        private set => _x = value;
    }

    public float Y
    {
        get => _y;
        private set => _y = value;
    }

    public float Width { get; protected set; }
    public float Height { get; protected set; }

    public int Rotation { get; private set; }

    public bool IsSelected { get; private set; }

    public bool IsPlaced => _placed;

    public bool CanPlaceAt(float x, float y) => x >= 0 && y >= 0;

    public void PlaceAt(float x, float y)
    {
        if (!CanPlaceAt(x, y))
            throw new ArgumentOutOfRangeException(nameof(x), "Position must be non-negative.");

        X = x;
        Y = y;
        _placed = true;
    }

    public void Rotate90()
    {
        (Width, Height) = (Height, Width);
        Rotation = (Rotation + 90) % 360;
    }

    public void Rotate() => Rotate90();

    public int GetRotationDegrees() => Rotation;

    public void Select() => IsSelected = true;

    public void Deselect() => IsSelected = false;

    public override RectangleF GetRenderBounds() => new(X, Y, Width, Height);

    internal void RestoreFromSnapshot(float x, float y, float width, float height, int rotation, bool placed)
    {
        if (width <= 0f || height <= 0f)
            throw new ArgumentOutOfRangeException(nameof(width));

        Width = width;
        Height = height;
        Rotation = (rotation % 360 + 360) % 360;
        _x = x;
        _y = y;
        _placed = placed;
    }
}
