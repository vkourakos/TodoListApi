using Ergasia2.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ergasia2.Api.Data;

public class Ergasia2DbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Todo> Todos { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }

    public Ergasia2DbContext()
    { }

    public Ergasia2DbContext(DbContextOptions<Ergasia2DbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Todo>()
            .HasMany(t => t.Items)
            .WithOne(i => i.Todo)
            .HasForeignKey(i => i.TodoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
