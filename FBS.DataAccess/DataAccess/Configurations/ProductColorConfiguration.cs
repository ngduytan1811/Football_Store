

namespace FBS.DataAccess.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using FBS.Infrastructure.Entities;

    public class ProductColorConfiguration : IEntityTypeConfiguration<ProductColor>
    {
        public void Configure(EntityTypeBuilder<ProductColor> builder)
        {
            builder.ToTable("ProductColors");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Color)
                .IsRequired()
                .HasMaxLength(50);

        
            builder.HasOne(x => x.Product)
                .WithMany(p => p.ProductColors)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

           
            builder.HasMany(x => x.ProductSizes)
                .WithOne(x => x.ProductColor)
                .HasForeignKey(x => x.ProductColorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}