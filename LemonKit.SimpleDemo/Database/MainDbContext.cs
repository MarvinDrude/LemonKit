
namespace LemonKit.SimpleDemo.Database;

public sealed class MainDbContext : DbContext
{

    public MainDbContext(DbContextOptions<MainDbContext> options)
        : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {



    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Pet>()
            .HasKey(p => p.Id);

    }

    public DbSet<Pet> Pets { get; set; }

}
