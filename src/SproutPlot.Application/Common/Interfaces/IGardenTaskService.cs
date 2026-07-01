using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Common.Results;
using SproutPlot.Application.Features.Tasks;
using SproutPlot.Application.Features.Tasks.Dtos;

namespace SproutPlot.Application.Common.Interfaces;

/// <summary>Use cases for managing garden tasks.</summary>
public interface IGardenTaskService
{
    /// <summary>Lists tasks across all the user's gardens (dashboard view).</summary>
    Task<PagedResult<TaskResponse>> GetTasksAsync(
        Guid ownerId,
        TaskQueryParameters query,
        CancellationToken cancellationToken = default);

    /// <summary>Lists tasks in a single garden the user owns.</summary>
    Task<Result<PagedResult<TaskResponse>>> GetTasksForGardenAsync(
        Guid gardenId,
        Guid ownerId,
        TaskQueryParameters query,
        CancellationToken cancellationToken = default);

    Task<Result<TaskResponse>> GetTaskAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task<Result<TaskResponse>> CreateTaskAsync(
        Guid gardenId,
        Guid ownerId,
        CreateTaskRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<TaskResponse>> UpdateTaskAsync(
        Guid id,
        Guid ownerId,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<TaskResponse>> CompleteTaskAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);

    Task<Result<TaskResponse>> SnoozeTaskAsync(
        Guid id,
        Guid ownerId,
        int days,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteTaskAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
}
