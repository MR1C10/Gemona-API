using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Infrastructure.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.Property(c => c.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Cpf)
                .HasConversion(
                    cpf => cpf.Valor,
                    valor => new Cpf(valor))
                .HasMaxLength(11)
                .IsRequired();

            builder.HasIndex(c => c.Cpf)
                .IsUnique();

            builder.Property(c => c.ImagemPerfilUrl)
                .HasMaxLength(500);

            builder.Property(c => c.DataCriacao)
                .IsRequired();

            builder.Property(c => c.Ativo)
                .HasDefaultValue(true);

            // Relacionamentos
            builder.HasOne(c => c.Endereco)
                .WithMany()
                .HasForeignKey(c => c.EnderecoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(c => c.Pedidos)
                .WithOne(p => p.Cliente)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Avaliacoes)
                .WithOne(a => a.Cliente)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}