namespace Clean.Architecture.Template.SharedKernel;

public interface IIdentifiable<T>
{
    public T Id { get; set; }
}