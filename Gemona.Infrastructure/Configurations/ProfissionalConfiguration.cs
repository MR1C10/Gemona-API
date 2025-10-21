using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Infrastructure.Configurations
{
    public class ProfissionalConfiguration : IEntityTypeConfiguration<Profissional>
    {
        public void Configure(EntityTypeBuilder<Profissional> builder)
        {
            builder.ToTable("Profissionais");

            builder.Property(p => p.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Cpf)
                .HasConversion(
                    cpf => cpf.Valor,
                    valor => new Cpf(valor))
                .HasMaxLength(11)
                .IsRequired();

            builder.HasIndex(p => p.Cpf)
                .IsUnique();

            builder.Property(p => p.ImagemPerfilUrl)
                .HasMaxLength(500);

            builder.Property(p => p.DataCriacao)
                .IsRequired();

            builder.Property(p => p.Ativo)
                .HasDefaultValue(true);

            // Relacionamentos
            builder.HasOne(p => p.Estabelecimento)
                .WithOne(e => e.Profissional)
                .HasForeignKey<Estabelecimento>(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}