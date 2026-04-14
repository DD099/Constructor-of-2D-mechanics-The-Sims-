using SimsConstructor.Models.Core;

namespace SimsConstructor.Models.Items;

public abstract class GameObject : IRenderable, IPricable, ISelectable
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _color = "#808080";
    private decimal _price;
    private bool _isSelected;

    public string Name
    {
        get => _name;
        protected set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Description
    {
        get => _description;
        protected set => _description = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Color
    {
        get => _color;
        protected set => _color = value ?? throw new ArgumentNullException(nameof(value));
    }

    public decimal Price => _price;

    public double Width { get; protected set; }

    public double Height { get; protected set; }

    public bool IsSelected => _isSelected;

    public abstract string RenderType { get; }

    public string DisplayName => Name;

    protected GameObject()
    {
    }

    protected GameObject(string name, string description, string color, decimal price, double width, double height)
    {
        Name = name;
        Description = description;
        Color = color;
        _price = price;
        Width = width;
        Height = height;
    }

    public abstract void Render();

    public void Select()
    {
        _isSelected = true;
        OnSelected();
    }

    public void Deselect()
    {
        _isSelected = false;
        OnDeselected();
    }

    protected virtual void OnSelected()
    {
    }

    protected virtual void OnDeselected()
    {
    }

    public override string ToString() => Name;
}
