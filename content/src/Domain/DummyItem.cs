using Clean.Architecture.Template.SharedKernel;

namespace Clean.Architecture.Template.Domain;

public class DummyItem : IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
