using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
                .Metadata.SetValueComparer(
                    new ValueComparer<Cpf>(
                        (c1, c2) => c1 != null && c2 != null && c1.Valor == c2.Valor,
                        c => c.Valor.GetHashCode(),
                        c => new Cpf(c.Valor)));

            builder.Property(p => p.Cpf)
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