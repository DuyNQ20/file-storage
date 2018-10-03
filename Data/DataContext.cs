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
            modelBuilder.Entity<Country>().ToTable("Country");
        }

        public DbSet<Country> Countries { get; set; }

        public DbSet<FileStorage.Models.FileSystem> FileStorage { get; set; }
    }
}
