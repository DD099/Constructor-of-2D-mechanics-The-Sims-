namespace SimsConstructor.Models.Interfaces;

public interface IRotatable
{
    void Rotate();
    int Rotation { get; }
    int GetRotationDegrees();
}
