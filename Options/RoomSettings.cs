namespace SimsConstructor.Options;

public sealed class RoomSettings
{
    public const string SectionName = "Room";

    public float WidthMeters { get; set; } = 6f;

    public float HeightMeters { get; set; } = 4f;

    public decimal? BudgetLimit { get; set; }
}
