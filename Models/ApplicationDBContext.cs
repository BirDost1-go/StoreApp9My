using Microsoft.EntityFrameworkCore;
namespace StoreApp9My.Models
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<People> Peoples { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.People)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.PeopleId);
            modelBuilder.Entity<People>()
                .HasIndex(p => p.Email)
                .IsUnique(); // Ensure unique email addresses
            modelBuilder.Entity<People>()
                .HasIndex(p => p.Phone)
                .IsUnique(); // Ensure unique phone numbers
                
        }
    }
}
