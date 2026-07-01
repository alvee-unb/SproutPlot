namespace SproutPlot.Application.Features.Calendar.Dtos;

/// <summary>What a calendar entry represents.</summary>
public enum CalendarEventKind
{
    Planting = 0,
    Task = 1,
    Watering = 2,
}

/// <summary>
/// A dated entry on the calendar, aggregated from existing data (plantings,
/// task due dates, watering history). This is a read model — no table backs it.
/// </summary>
public sealed record CalendarEvent
{
    public required DateOnly Date { get; init; }

    public required CalendarEventKind Kind { get; init; }

    public required string Title { get; init; }

    public required Guid GardenId { get; init; }

    public string? GardenName { get; init; }

    public Guid? PlantId { get; init; }

    public string? PlantName { get; init; }

    /// <summary>Extra context, e.g. task status or watering amount.</summary>
    public string? Detail { get; init; }

    /// <summary>Id of the source entity (task, plant or watering) for linking.</summary>
    public required Guid SourceId { get; init; }
}
