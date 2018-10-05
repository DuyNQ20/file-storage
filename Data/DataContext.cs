using FileStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Data
{
    public class DataContext : DbContext
    {
        public DbContextOptions<DataContext> Options { get; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Options = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Files>()
            .HasKey(c => new { c.ID });
            modelBuilder.Entity<Files>().ToTable("Files");
        }
        

        public DbSet<FileStorage.Models.Files> Files { get; set; }
    }
}
