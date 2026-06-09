using Microsoft.EntityFrameworkCore;
using RubricaApi.Models;

namespace RubricaApi.Models;

public class ContattoContext : DbContext
{
    public ContattoContext(DbContextOptions<ContattoContext> options) : base(options)
    {
    }

    public DbSet<Contatto> Contatti { get; set; }
}