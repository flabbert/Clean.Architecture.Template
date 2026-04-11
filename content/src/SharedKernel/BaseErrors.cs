namespace Clean.Architecture.Template.SharedKernel;

public abstract class BaseErrors<T>
{
    protected static string ErrorGroup => typeof(T).Name;

    public static Error NotFound() => Error.NotFound($"{ErrorGroup}.NotFound", $"the {ErrorGroup} was not found");
    public static Error NotFound(Guid identifier) => NotFound(identifier.ToString());
    public static Error NotFound(ulong identifier) => NotFound(identifier.ToString());
    public static Error NotFound(string identifier) => NotFound(nameof(NotFound), identifier);
    public static Error NotFound(string type, string identifier)
    {
        var error = Error.NotFound($"{ErrorGroup}.{type}", $"the {ErrorGroup} with id: {identifier} was not found");
        error.Data.Add("identifier", identifier);
        return error;
    }

    public static Error AlreadyExists() => Conflict("AlreadyExists", $"the entity of type {ErrorGroup} already exists");

    public static Error AlreadyExists(Guid identifier) => AlreadyExists(identifier.ToString());
    public static Error AlreadyExists(ulong identifier) => AlreadyExists(identifier.ToString());
    public static Error AlreadyExists(string identifier) => AlreadyExists(nameof(AlreadyExists), identifier);
    public static Error AlreadyExists(string type, string identifier)
    {
        var error = Error.Conflict($"{ErrorGroup}.{type}", $" type {typeof(T).Name} with identifiers: {identifier} already exists");
        error.Data.Add("identifier", identifier);
        return error;
    }

    public static Error Failure(string type, string message) => Error.Failure($"{ErrorGroup}.{type}", message);
    public static Error Conflict(string type, string message) => Error.Conflict($"{ErrorGroup}.{type}", message);
    public static Error Problem(string type, string message) => Error.Problem($"{ErrorGroup}.{type}", message);
}