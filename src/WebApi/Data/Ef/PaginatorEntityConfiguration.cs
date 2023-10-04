namespace WebApi.Data.Ef;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PaginatorEntityConfiguration : IEntityTypeConfiguration<PaginatorEntity>
{
    public void Configure(EntityTypeBuilder<PaginatorEntity> builder)
    {
        builder
            .ToTable("Paginators");

        builder.HasIndex(e => e.Id).IsUnique();
    }
}
