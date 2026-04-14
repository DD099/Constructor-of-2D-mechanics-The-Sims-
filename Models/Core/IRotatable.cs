namespace SimsConstructor.Models.Core;

public interface IRotatable
{
    double RotationAngle { get; set; }
    void RotateClockwise();
    void RotateCounterClockwise();
}
