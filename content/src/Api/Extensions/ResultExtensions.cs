using Clean.Architecture.Template.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace Clean.Architecture.Template.Api.Extensions;
public static class ResultExtensions
{
    public static ObjectResult ToHttpResponse<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new ObjectResult(result.Error)
            {
                StatusCode = (int)result.Error.Code
            };
    }
    public static ObjectResult ToHttpResponse(this Result result)
    {
        return result.IsSuccess
            ? new OkObjectResult(null)
            : new ObjectResult(result.Error)
            {
                StatusCode = (int)result.Error.Code
            };
    }
    public static async ValueTask<ObjectResult> ToHttpResponse<T>(this ValueTask<Result<T>> task) where T : notnull
    {
        var result = await task;
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new ObjectResult(result.Error)
            {
                StatusCode = (int)result.Error.Code
            };
    }
    public static async ValueTask<ObjectResult> ToHttpResponse(this ValueTask<Result> task)
    {
        var result = await task;
        return result.IsSuccess
            ? new OkObjectResult(null)
            : new ObjectResult(result.Error)
            {
                StatusCode = (int)result.Error.Code
            };
    }
}
