using Microsoft.EntityFrameworkCore;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Features.Calendar.Dtos;

namespace SproutPlot.Infrastructure.Persistence.Repositories;

/// <summary>
/// Builds the calendar read model by unioning three owner-scoped, date-bounded
/// queries: plantings, task due dates, and watering history.
/// </summary>
public sealed class CalendarRepository : ICalendarRepository
{
    private readonly AppDbContext _db;

    public CalendarRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetEventsAsync(
        Guid ownerId,
        DateOnly from,
        DateOnly to,
        Guid? gardenId,
        CancellationToken cancellationToken = default)
    {
        var events = new List<CalendarEvent>();

        // --- Plantings (Plant.DatePlanted within range) ---
        var plantings = await _db.Plants
            .AsNoTracking()
            .Where(p => p.Garden!.OwnerId == ownerId
                && p.DatePlanted != null
                && p.DatePlanted >= from
                && p.DatePlanted <= to
                && (gardenId == null || p.GardenId == gardenId))
            .Select(p => new
            {
                p.Id,
                p.Name,
                TypeName = p.PlantType != null ? p.PlantType.Name : null,
                Date = p.DatePlanted!.Value,
                p.GardenId,
                GardenName = p.Garden!.Name,
            })
            .ToListAsync(cancellationToken);

        events.AddRange(plantings.Select(p => new CalendarEvent
        {
            Date = p.Date,
            Kind = CalendarEventKind.Planting,
            Title = $"Planted {p.TypeName ?? p.Name}",
            GardenId = p.GardenId,
            GardenName = p.GardenName,
            PlantId = p.Id,
            PlantName = p.Name,
            SourceId = p.Id,
        }));

        // --- Tasks (GardenTask.DueOn within range) ---
        var tasks = await _db.GardenTasks
            .AsNoTracking()
            .Where(t => t.Garden!.OwnerId == ownerId
                && t.DueOn >= from
                && t.DueOn <= to
                && (gardenId == null || t.GardenId == gardenId))
            .Select(t => new
            {
                t.Id,
                t.Type,
                t.Title,
                t.Status,
                t.DueOn,
                t.GardenId,
                GardenName = t.Garden!.Name,
                t.PlantId,
                PlantName = t.Plant != null ? t.Plant.Name : null,
            })
            .ToListAsync(cancellationToken);

        events.AddRange(tasks.Select(t => new CalendarEvent
        {
            Date = t.DueOn,
            Kind = CalendarEventKind.Task,
            Title = string.IsNullOrWhiteSpace(t.Title) ? t.Type.ToString() : t.Title!,
            GardenId = t.GardenId,
            GardenName = t.GardenName,
            PlantId = t.PlantId,
            PlantName = t.PlantName,
            Detail = t.Status.ToString(),
            SourceId = t.Id,
        }));

        // --- Waterings (WateringEvent.WateredAtUtc date within range) ---
        var fromUtc = DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var toExclusiveUtc = DateTime.SpecifyKind(to.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        var waterings = await _db.WateringEvents
            .AsNoTracking()
            .Where(w => w.Garden!.OwnerId == ownerId
                && w.WateredAtUtc >= fromUtc
                && w.WateredAtUtc < toExclusiveUtc
                && (gardenId == null || w.GardenId == gardenId))
            .Select(w => new
            {
                w.Id,
                w.WateredAtUtc,
                w.AmountLiters,
                w.GardenId,
                GardenName = w.Garden!.Name,
                w.PlantId,
                PlantName = w.Plant != null ? w.Plant.Name : null,
            })
            .ToListAsync(cancellationToken);

        events.AddRange(waterings.Select(w => new CalendarEvent
        {
            Date = DateOnly.FromDateTime(w.WateredAtUtc),
            Kind = CalendarEventKind.Watering,
            Title = "Watered",
            GardenId = w.GardenId,
            GardenName = w.GardenName,
            PlantId = w.PlantId,
            PlantName = w.PlantName,
            Detail = w.AmountLiters is { } litres ? $"{litres} L" : null,
            SourceId = w.Id,
        }));

        return events;
    }
}
