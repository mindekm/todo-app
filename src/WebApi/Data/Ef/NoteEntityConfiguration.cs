namespace WebApi.Data.Ef;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class NoteEntityConfiguration : IEntityTypeConfiguration<NoteEntity>
{
    public void Configure(EntityTypeBuilder<NoteEntity> builder)
    {
        builder
            .ToTable("Notes")
            .Property(e => e.ModifiedAt).IsConcurrencyToken();

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.ExternalId).IsUnique();
        builder.HasIndex(e => e.CreatedBy);

        builder
            .HasGeneratedTsVectorColumn(
                e => e.SearchVector,
                "english",
                e => new
                {
                    e.Title,
                    e.Content,
                })
            .HasIndex(e => e.SearchVector)
            .HasMethod("GIN");
    }
}
