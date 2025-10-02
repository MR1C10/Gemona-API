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
            builder.ToTable("Cliente");

            builder.HasKey(c => c.ClienteId);
            builder.Property(c => c.ClienteId).HasColumnName("cliente_id");

            builder.Property(c => c.Nome)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.Email)
                .HasColumnName("email")
                .HasMaxLength(150)
                .IsRequired();

            builder.HasIndex(c => c.Email).IsUnique();

            builder.Property(c => c.Telefone)
                .HasColumnName("telefone")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.Cpf)
                .HasColumnName("cpf")
                .HasMaxLength(11)
                .HasConversion(
                    cpf => cpf.Valor,
                    valor => new Cpf(valor)
                )
                .IsRequired();

            builder.HasIndex(c => c.Cpf).IsUnique();

            builder.Property(c => c.ImagemPerfilUrl)
                .HasColumnName("imagem_perfil_url")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.EnderecoId)
                .HasColumnName("endereco_id");

            builder.Property(c => c.DataNacimento)
                .HasColumnName("data_nacimento")
                .HasColumnType("DATE")
                .IsRequired();

            builder.Property(c => c.SenhaHash)
                .HasColumnName("senha_hash")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(c => c.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(c => c.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("DATETIME(6)");

            builder.Property(c => c.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne(c => c.Endereco)
                .WithOne(e => e.Cliente)
                .HasForeignKey<Cliente>(c => c.EnderecoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Pedidos)
                .WithOne(p => p.Cliente)
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Avaliacoes)
                .WithOne(a => a.Cliente)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}