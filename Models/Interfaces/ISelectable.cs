namespace SimsConstructor.Models.Interfaces;

public interface ISelectable
{
    void Select();
    void Deselect();
    bool IsSelected { get; }
}
