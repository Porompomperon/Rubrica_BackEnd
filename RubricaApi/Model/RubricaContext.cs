using Microsoft.EntityFrameworkCore;
using RubricaApi.Models;

namespace RubricaApi.Model;

public class RubricaContext : DbContext
{
    public RubricaContext(DbContextOptions<RubricaContext> options) : base(options)
    {
    }

    public DbSet<Contatto> Contatti { get; set; }
    public DbSet<Familiare> Familiari { get; set; }

}