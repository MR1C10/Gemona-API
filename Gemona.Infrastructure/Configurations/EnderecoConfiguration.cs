using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Infrastructure.Configurations
{
    public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("endereco");

            builder.HasKey(e => e.EnderecoId);
            builder.Property(e => e.EnderecoId).HasColumnName("endereco_id");

            builder.Property(e => e.Rua)
                .HasColumnName("rua")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(e => e.Numero)
                .HasColumnName("numero")
                .HasMaxLength(20);

            builder.Property(e => e.Bairro)
                .HasColumnName("bairro")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Complemento)
                .HasColumnName("complemento")
                .HasMaxLength(100);

            builder.Property(e => e.Cidade)
                .HasColumnName("cidade")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Estado)
                .HasColumnName("estado")
                .HasMaxLength(2)
                .IsRequired();

            // Configuração do Value Object CEP
            builder.Property(e => e.Cep)
                .HasColumnName("cep")
                .HasMaxLength(8)
                .HasConversion(
                    cep => cep.Valor,
                    valor => new Cep(valor))
                .IsRequired();

            builder.Property(e => e.Latitude)
                .HasColumnName("latitude")
                .HasPrecision(10, 8)
                .IsRequired();

            builder.Property(e => e.Longitude)
                .HasColumnName("longitude")
                .HasPrecision(11, 8)
                .IsRequired();

            // Configurações de auditoria
            builder.Property(e => e.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(e => e.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("DATETIME(6)");

            builder.Property(e => e.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne(e => e.Estabelecimento)
                .WithOne(est => est.Endereco)
                .HasForeignKey<Estabelecimento>(est => est.EnderecoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Cliente)
                .WithOne(c => c.Endereco)
                .HasForeignKey<Cliente>(c => c.EnderecoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}