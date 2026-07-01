using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Tasks.Dtos;
using SproutPlot.Domain.Entities;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Application.Features.Tasks;

/// <summary>Orchestrates garden task use cases; ownership is enforced per garden.</summary>
public sealed class GardenTaskService : IGardenTaskService
{
    private readonly IGardenTaskRepository _tasks;
    private readonly IGardenRepository _gardens;
    private readonly IPlantRepository _plants;
    private readonly TimeProvider _timeProvider;

    public GardenTaskService(
        IGardenTaskRepository tasks,
        IGardenRepository gardens,
        IPlantRepository plants,
        TimeProvider timeProvider)
    {
        _tasks = tasks;
        _gardens = gardens;
        _plants = plants;
        _timeProvider = timeProvider;
    }

    public async Task<PagedResult<TaskResponse>> GetTasksAsync(
        Guid ownerId,
        TaskQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var page = await _tasks.GetPagedForOwnerAsync(ownerId, query, cancellationToken);
        return MapPage(page);
    }

    public async Task<Result<PagedResult<TaskResponse>>> GetTasksForGardenAsync(
        Guid gardenId,
        Guid ownerId,
        TaskQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetByIdForOwnerAsync(gardenId, ownerId, cancellationToken);
        if (garden is null)
        {
            return Result<PagedResult<TaskResponse>>.NotFound("Garden not found.");
        }

        var page = await _tasks.GetPagedForOwnerAsync(ownerId, query with { GardenId = gardenId }, cancellationToken);
        return Result<PagedResult<TaskResponse>>.Success(MapPage(page));
    }

    public async Task<Result<TaskResponse>> GetTaskAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        return task is null
            ? Result<TaskResponse>.NotFound("Task not found.")
            : Result<TaskResponse>.Success(MapToResponse(task));
    }

    public async Task<Result<TaskResponse>> CreateTaskAsync(
        Guid gardenId,
        Guid ownerId,
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var garden = await _gardens.GetByIdForOwnerAsync(gardenId, ownerId, cancellationToken);
        if (garden is null)
        {
            return Result<TaskResponse>.NotFound("Garden not found.");
        }

        if (request.PlantId is { } plantId)
        {
            var plant = await _plants.GetByIdForOwnerAsync(plantId, ownerId, cancellationToken);
            if (plant is null || plant.GardenId != gardenId)
            {
                return Result<TaskResponse>.Failure("The specified plant is not in this garden.");
            }
        }

        var task = new GardenTask
        {
            GardenId = gardenId,
            PlantId = request.PlantId,
            Type = request.Type,
            Title = request.Title?.Trim(),
            DueOn = request.DueOn,
            Status = GardenTaskStatus.Pending,
            Notes = request.Notes?.Trim(),
        };

        await _tasks.AddAsync(task, cancellationToken);

        var created = await _tasks.GetByIdForOwnerAsync(task.Id, ownerId, cancellationToken);
        return Result<TaskResponse>.Success(MapToResponse(created ?? task));
    }

    public async Task<Result<TaskResponse>> UpdateTaskAsync(
        Guid id,
        Guid ownerId,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        if (task is null)
        {
            return Result<TaskResponse>.NotFound("Task not found.");
        }

        task.Type = request.Type;
        task.Title = request.Title?.Trim();
        task.DueOn = request.DueOn;
        task.Notes = request.Notes?.Trim();

        await _tasks.UpdateAsync(task, cancellationToken);
        return Result<TaskResponse>.Success(MapToResponse(task));
    }

    public async Task<Result<TaskResponse>> CompleteTaskAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        if (task is null)
        {
            return Result<TaskResponse>.NotFound("Task not found.");
        }

        task.Status = GardenTaskStatus.Completed;
        task.CompletedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;

        await _tasks.UpdateAsync(task, cancellationToken);
        return Result<TaskResponse>.Success(MapToResponse(task));
    }

    public async Task<Result<TaskResponse>> SnoozeTaskAsync(
        Guid id,
        Guid ownerId,
        int days,
        CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        if (task is null)
        {
            return Result<TaskResponse>.NotFound("Task not found.");
        }

        // Push from today (or the existing due date, if it is already in the future).
        var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        var basis = task.DueOn > today ? task.DueOn : today;
        task.DueOn = basis.AddDays(days);
        task.Status = GardenTaskStatus.Pending;
        task.CompletedAtUtc = null;

        await _tasks.UpdateAsync(task, cancellationToken);
        return Result<TaskResponse>.Success(MapToResponse(task));
    }

    public async Task<Result> DeleteTaskAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdForOwnerAsync(id, ownerId, cancellationToken);
        if (task is null)
        {
            return Result.NotFound("Task not found.");
        }

        await _tasks.DeleteAsync(task, cancellationToken);
        return Result.Success();
    }

    private static PagedResult<TaskResponse> MapPage(PagedResult<GardenTask> page) => new()
    {
        Items = page.Items.Select(MapToResponse).ToList(),
        Page = page.Page,
        PageSize = page.PageSize,
        TotalCount = page.TotalCount,
    };

    private static TaskResponse MapToResponse(GardenTask task) => new()
    {
        Id = task.Id,
        GardenId = task.GardenId,
        GardenName = task.Garden?.Name,
        PlantId = task.PlantId,
        PlantName = task.Plant?.Name,
        Type = task.Type,
        Title = task.Title,
        DueOn = task.DueOn,
        Status = task.Status,
        CompletedAtUtc = task.CompletedAtUtc,
        Notes = task.Notes,
        CreatedAtUtc = task.CreatedAtUtc,
    };
}
