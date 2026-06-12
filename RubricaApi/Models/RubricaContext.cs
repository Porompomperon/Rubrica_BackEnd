using Microsoft.EntityFrameworkCore;
using RubricaApi.Models;

namespace RubricaApi.Data;

public class RubricaContext : DbContext
{
    public RubricaContext(DbContextOptions<RubricaContext> options)
        : base(options) { }

    public DbSet<Contatto> Contatti { get; set; } = null!;
    public DbSet<Famigliare> Famigliari { get; set; } = null!;
    public DbSet<Utente> Utenti { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<Famigliare>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
            
        modelBuilder.Entity<Famigliare>()
            .HasKey(f => new { f.ContattoId, f.FamigliareId });

        modelBuilder.Entity<Famigliare>()
            .HasOne<Contatto>()
            .WithMany()
            .HasForeignKey(f => f.ContattoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Famigliare>()
            .HasOne<Contatto>()
            .WithMany()
            .HasForeignKey(f => f.FamigliareId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}