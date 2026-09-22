using Clean.Architecture.Template.SharedKernel;

namespace Clean.Architecture.Template.Api.Extensions;

// Returns IResult so the same extensions work in controller actions and minimal API endpoints.
public static class ResultExtensions
{
    public static IResult ToHttpResponse<T>(this Result<T> result)
        => result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Json(result.Error, statusCode: (int)result.Error.Code);

    public static IResult ToHttpResponse(this Result result)
        => result.IsSuccess
            ? Results.Ok()
            : Results.Json(result.Error, statusCode: (int)result.Error.Code);

    public static async ValueTask<IResult> ToHttpResponse<T>(this ValueTask<Result<T>> task) where T : notnull
        => (await task).ToHttpResponse();

    public static async ValueTask<IResult> ToHttpResponse(this ValueTask<Result> task)
        => (await task).ToHttpResponse();
}
