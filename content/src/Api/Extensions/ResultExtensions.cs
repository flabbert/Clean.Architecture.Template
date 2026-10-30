using Clean.Architecture.Template.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace Clean.Architecture.Template.Api.Extensions;

// Returns IResult so the same extensions work in controller actions and minimal API endpoints.
public static class ResultExtensions
{
    public static ObjectResult ToHttpResponse<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new ObjectResult(result.Error.ToProblemDetails())
            {
                StatusCode = (int)result.Error.Code
            };
    }
    public static ObjectResult ToHttpResponse(this Result result)
    {
        return result.IsSuccess
            ? new OkObjectResult(null)
            : new ObjectResult(result.Error.ToProblemDetails())
            {
                StatusCode = (int)result.Error.Code
            };
    }
    public static async ValueTask<ObjectResult> ToHttpResponse<T>(this ValueTask<Result<T>> task) where T : notnull
    {
        var result = await task;
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new ObjectResult(result.Error.ToProblemDetails())
            {
                StatusCode = (int)result.Error.Code
            };
    }
    public static async ValueTask<ObjectResult> ToHttpResponse(this ValueTask<Result> task)
    {
        var result = await task;
        return result.IsSuccess
            ? new OkObjectResult(null)
            : new ObjectResult(result.Error.ToProblemDetails())
            {
                StatusCode = (int)result.Error.Code
            };
    }

    public static ProblemDetails ToProblemDetails(this Error error)
    {
        var problem = new ProblemDetails();

        problem.Detail = error.Description;
        problem.Status = (int)error.Code;
        problem.Extensions = error.Data;
        problem.Type = error.Type;
        problem.Title = error.Type;


        return problem;
    }
}
