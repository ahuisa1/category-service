using CategoryService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .ToContainer("Categories") // Nombre del contenedor en Cosmos DB
                .HasPartitionKey(c => c.Id); // Usamos el Id como Partition Key para simplicidad
        }
    }
}
