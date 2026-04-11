using Clean.Architecture.Template.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clean.Architecture.Template.Infrastructure.Persistence.Configurations;

public class DummyItemConfiguration : IEntityTypeConfiguration<DummyItem>
{
    public void Configure(EntityTypeBuilder<DummyItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
    }
}
