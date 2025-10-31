using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;

namespace Gemona.Infrastructure.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.ToTable("Admins");

            builder.Property(a => a.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.GithubUsername)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(a => a.GithubUsername)
                .IsUnique();

            builder.Property(a => a.DataCriacao)
                .IsRequired();

            builder.Property(a => a.Ativo)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
