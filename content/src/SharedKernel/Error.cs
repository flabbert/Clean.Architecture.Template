using System.Net;
using System.Text.Json.Serialization;

namespace Clean.Architecture.Template.SharedKernel;

public record Error
{
    // ReSharper disable once CollectionNeverQueried.Global
    public Dictionary<string, object> Data { get; set; } = [];
    private const string Category = "General";
    public static readonly Error None = new($"{Category}.None", string.Empty, ErrorCategory.Failure, HttpStatusCode.OK);
    public static readonly Error NullValue = new($"{Category}.Null", "Null value was provided", ErrorCategory.Failure, HttpStatusCode.UnprocessableEntity);
    protected Error(string type, string description, ErrorCategory errorCategory, HttpStatusCode code)
    {
        Type = type;
        Description = description;
        ErrorCategory = errorCategory;
        Code = code;

    }
    public string Type { get; }
    public string Description { get; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorCategory ErrorCategory { get; }
    public HttpStatusCode Code { get; init; }
    public static implicit operator int(Error error) => (int)error.Code;
    public static Error NotFound(string code, string description, HttpStatusCode statusCode = HttpStatusCode.NotFound) => new(code, description, ErrorCategory.NotFound, statusCode);
    public static Error Failure(string code, string description, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) => new(code, description, ErrorCategory.Failure, statusCode);
    public static Error Problem(string code, string description, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) => new(code, description, ErrorCategory.Problem, statusCode);
    public static Error Conflict(string code, string description, HttpStatusCode statusCode = HttpStatusCode.Conflict) => new(code, description, ErrorCategory.Conflict, statusCode);
}
