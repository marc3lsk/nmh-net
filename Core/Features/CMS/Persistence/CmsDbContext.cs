using Core.Features.CMS.Domain;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.CMS.Persistence;

public class CmsDbContext : DbContext
{
    public CmsDbContext(DbContextOptions<CmsDbContext> options)
        : base(options) { }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Author
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Name).IsRequired();

            entity.HasIndex(a => a.Name).IsUnique();

            entity
                .HasOne(a => a.Image)
                .WithOne()
                .HasForeignKey<Author>(a => a.Id) // Configures Author.Id as the foreign key
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Image
        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Description);
        });

        // Article
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Title).IsRequired();
            entity.HasIndex(a => a.Title);
            entity.HasMany(a => a.Author).WithMany();
            entity.HasOne(a => a.Site).WithMany().HasForeignKey("SiteId");
        });

        // Site
        modelBuilder.Entity<Site>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.CreatedAt).IsRequired();
        });
    }
}
