namespace SproutPlot.Application.Common.Interfaces;

/// <summary>
/// Seam for premium, AI-powered gardening guidance.
///
/// This interface is intentionally defined in the Application layer and has no
/// implementation yet. Deterministic business rules (watering, reminders,
/// scheduling) must never depend on it; AI is an additive consumer of already
/// computed, structured application data — never a replacement for those rules.
///
/// When premium AI is implemented, the concrete adapter lives in Infrastructure
/// (e.g. an Azure OpenAI client) and receives a structured context object rather
/// than raw user prompts. The context type will be introduced alongside the
/// garden/weather features that supply its data.
/// </summary>
public interface IAiAdvisor
{
    /// <summary>
    /// Produces gardening advice from structured application data.
    /// </summary>
    /// <param name="context">
    /// Structured, server-assembled context (location, weather, garden, plants,
    /// history, season). Never a free-form end-user prompt.
    /// </param>
    Task<string> GetAdviceAsync(object context, CancellationToken cancellationToken = default);
}
