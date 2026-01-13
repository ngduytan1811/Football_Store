

namespace FBS.DataAccess.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using FBS.Infrastructure.Entities;

    public class ProductSizeConfiguration : IEntityTypeConfiguration<ProductSize>
    {
        public void Configure(EntityTypeBuilder<ProductSize> builder)
        {
            builder.ToTable("ProductSizes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Size)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(x => x.Quantity)
                .IsRequired();

            builder.HasOne(x => x.ProductColor)
                .WithMany(x => x.ProductSizes)
                .HasForeignKey(x => x.ProductColorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}