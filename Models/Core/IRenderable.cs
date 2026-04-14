namespace SimsConstructor.Models.Core;

public interface IRenderable
{
    string RenderType { get; }
    void Render();
}
