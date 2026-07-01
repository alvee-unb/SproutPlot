using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SproutPlot.Application.Common.Interfaces;
using SproutPlot.Application.Common.Models;
using SproutPlot.Application.Features.Tasks;
using SproutPlot.Application.Features.Tasks.Dtos;
using SproutPlot.Domain.Enums;

namespace SproutPlot.Api.Controllers;

/// <summary>Garden task endpoints: create (nested), list, edit, complete, snooze, delete.</summary>
[Authorize]
[Produces("application/json")]
public sealed class TasksController : ApiControllerBase
{
    private readonly IGardenTaskService _taskService;
    private readonly IValidator<CreateTaskRequest> _createValidator;
    private readonly IValidator<UpdateTaskRequest> _updateValidator;
    private readonly IValidator<SnoozeTaskRequest> _snoozeValidator;

    public TasksController(
        IGardenTaskService taskService,
        IValidator<CreateTaskRequest> createValidator,
        IValidator<UpdateTaskRequest> updateValidator,
        IValidator<SnoozeTaskRequest> snoozeValidator)
    {
        _taskService = taskService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _snoozeValidator = snoozeValidator;
    }

    /// <summary>Lists tasks across all the user's gardens (for the dashboard).</summary>
    [HttpGet("api/tasks")]
    [ProducesResponseType(typeof(PagedResult<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GardenTaskStatus? status = null,
        [FromQuery] DateOnly? dueOnOrBefore = null,
        [FromQuery] TaskSortBy sortBy = TaskSortBy.DueOn,
        [FromQuery] bool descending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = TaskQueryParameters.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(status, dueOnOrBefore, sortBy, descending, page, pageSize);
        var result = await _taskService.GetTasksAsync(CurrentUserId, query, cancellationToken);
        return Ok(result);
    }

    /// <summary>Lists tasks in a single garden.</summary>
    [HttpGet("api/gardens/{gardenId:guid}/tasks")]
    [ProducesResponseType(typeof(PagedResult<TaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByGarden(
        Guid gardenId,
        [FromQuery] GardenTaskStatus? status = null,
        [FromQuery] DateOnly? dueOnOrBefore = null,
        [FromQuery] TaskSortBy sortBy = TaskSortBy.DueOn,
        [FromQuery] bool descending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = TaskQueryParameters.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(status, dueOnOrBefore, sortBy, descending, page, pageSize);
        var result = await _taskService.GetTasksForGardenAsync(gardenId, CurrentUserId, query, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Creates a task in a garden.</summary>
    [HttpPost("api/gardens/{gardenId:guid}/tasks")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid gardenId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var result = await _taskService.CreateTaskAsync(gardenId, CurrentUserId, request, cancellationToken);
        if (!result.Succeeded)
        {
            return FromError(result);
        }

        return CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Gets a single task.</summary>
    [HttpGet("api/tasks/{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _taskService.GetTaskAsync(id, CurrentUserId, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Edits a task's details.</summary>
    [HttpPut("api/tasks/{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var result = await _taskService.UpdateTaskAsync(id, CurrentUserId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Marks a task complete.</summary>
    [HttpPost("api/tasks/{id:guid}/complete")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _taskService.CompleteTaskAsync(id, CurrentUserId, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Snoozes a task by pushing its due date forward.</summary>
    [HttpPost("api/tasks/{id:guid}/snooze")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Snooze(Guid id, SnoozeTaskRequest request, CancellationToken cancellationToken)
    {
        var validation = await _snoozeValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var result = await _taskService.SnoozeTaskAsync(id, CurrentUserId, request.Days, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : FromError(result);
    }

    /// <summary>Deletes a task.</summary>
    [HttpDelete("api/tasks/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _taskService.DeleteTaskAsync(id, CurrentUserId, cancellationToken);
        return result.Succeeded ? NoContent() : FromError(result);
    }

    private static TaskQueryParameters BuildQuery(
        GardenTaskStatus? status,
        DateOnly? dueOnOrBefore,
        TaskSortBy sortBy,
        bool descending,
        int page,
        int pageSize) => new()
    {
        Status = status,
        DueOnOrBefore = dueOnOrBefore,
        SortBy = sortBy,
        Descending = descending,
        Page = page,
        PageSize = pageSize,
    };
}
