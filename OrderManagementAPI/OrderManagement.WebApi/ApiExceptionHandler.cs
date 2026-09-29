using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OrderManagement.Application.Common.Exceptions;

namespace OrderManagement.WebApi;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            ValidationException validationException => CreateValidationProblem(validationException),
            NotFoundException notFoundException => CreateProblem(StatusCodes.Status404NotFound, "Not found", notFoundException.Message),
            ConflictException conflictException => CreateProblem(StatusCodes.Status409Conflict, "Conflict", conflictException.Message),
            BusinessRuleException businessRuleException => CreateProblem(StatusCodes.Status400BadRequest, "Business rule violation", businessRuleException.Message),
            PostgresException { SqlState: Npgsql.PostgresErrorCodes.UniqueViolation } => CreateProblem(StatusCodes.Status409Conflict, "Conflict", "A record with the same unique value already exists."),
            PostgresException { SqlState: Npgsql.PostgresErrorCodes.ForeignKeyViolation } => CreateProblem(StatusCodes.Status409Conflict, "Conflict", "The request references a record that cannot be used."),
            _ => CreateProblem(StatusCodes.Status500InternalServerError, "Internal server error", "An unexpected error occurred.")
        };

        if (problem.Status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled API exception. TraceId: {TraceId}", httpContext.TraceIdentifier);
        }

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static ProblemDetails CreateValidationProblem(ValidationException exception)
    {
        var problem = CreateProblem(
            StatusCodes.Status400BadRequest,
            "Validation failed",
            "One or more validation errors occurred.");
        problem.Extensions["errors"] = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());
        return problem;
    }

    private static ProblemDetails CreateProblem(int status, string title, string detail)
    {
        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };
    }
}