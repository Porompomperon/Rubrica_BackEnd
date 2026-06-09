using Microsoft.EntityFrameworkCore;
using RubricaApi.Models;

namespace RubricaApi.Data;

public class RubricaContext : DbContext
{
    public RubricaContext(DbContextOptions<RubricaContext> options)
        : base(options) { }

    public DbSet<Contatto> Contatti { get; set; } = null!;
    public DbSet<Familiare> Familiari { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Familiare>()
            .HasKey(f => new { f.ContattoId, f.FamiliareId });

        modelBuilder.Entity<Familiare>()
            .HasOne<Contatto>()
            .WithMany()
            .HasForeignKey(f => f.ContattoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Familiare>()
            .HasOne<Contatto>()
            .WithMany()
            .HasForeignKey(f => f.FamiliareId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}