namespace SimsConstructor.Models.Core;

public interface IPricable
{
    decimal Price { get; }
    string DisplayName { get; }
}
