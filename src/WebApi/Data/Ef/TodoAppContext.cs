namespace WebApi.Data.Ef;

using Microsoft.EntityFrameworkCore;

public sealed class TodoAppContext(DbContextOptions<TodoAppContext> options) : DbContext(options)
{
    public DbSet<NoteEntity> Notes => Set<NoteEntity>();

    public DbSet<PaginatorEntity> Paginators => Set<PaginatorEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoAppContext).Assembly);
    }
}
