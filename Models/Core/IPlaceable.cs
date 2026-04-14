namespace SimsConstructor.Models.Core;

public interface IPlaceable
{
    double PositionX { get; set; }
    double PositionY { get; set; }
    bool IsPlaced { get; }
    bool PlaceAt(double x, double y);
}
