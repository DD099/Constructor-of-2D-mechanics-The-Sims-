namespace SimsConstructor.Models.Interfaces;

public interface IPlaceable
{
    bool CanPlaceAt(float x, float y);
    void PlaceAt(float x, float y);
    bool IsPlaced { get; }
}
