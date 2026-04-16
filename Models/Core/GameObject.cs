using System.Drawing;

namespace SimsConstructor.Models.Core;

/// <summary>
/// Abstract base for all entities in the designer.
/// </summary>
public abstract class GameObject
{
    protected GameObject(string name, string description, Color color)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Color = color;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Color Color { get; }

    public abstract RectangleF GetRenderBounds();

    public override string ToString() => Name;
}
