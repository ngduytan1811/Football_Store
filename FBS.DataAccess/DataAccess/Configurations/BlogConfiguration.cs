
namespace FBS.DataAccess.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using FBS.Infrastructure.Entities;

    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(255);
            builder.Property(x => x.Thumbnail).HasMaxLength(255);
            builder.Property(x => x.Author).HasMaxLength(255);
        }
    }
}