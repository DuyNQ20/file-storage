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
            modelBuilder.Entity<FileSystem>()
            .HasKey(c => new { c.ID });
            modelBuilder.Entity<FileSystem>().ToTable("FileSystem");
        }
        

        public DbSet<FileStorage.Models.FileSystem> FileSystem { get; set; }
    }
}
