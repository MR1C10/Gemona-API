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
            builder.ToTable("profissional");

            builder.HasKey(p => p.ProfissionalId);

            builder.Property(p => p.ProfissionalId).HasColumnName("profissional_id");

            builder.Property(p => p.Nome)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Email)
                .HasColumnName("email")
                .HasMaxLength(150);

            builder.HasIndex(p => p.Email).IsUnique();

            builder.Property(p => p.Telefone)
                .HasColumnName("telefone")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.Cpf)
                .HasColumnName("cpf")
                .HasMaxLength(11)
                .HasConversion(
                    cpf => cpf.Valor,
                    valor => new Cpf(valor)
                )
                .IsRequired();

            builder.HasIndex(p => p.Cpf).IsUnique();

            builder.Property(p => p.ImagemPerfilUrl)
                .HasColumnName("imagem_perfil_url")
                .HasMaxLength(255);

            builder.Property(p => p.DataNacimento)
                .HasColumnName("data_nacimento")
                .HasColumnType("DATE")
                .IsRequired();

            builder.Property(p => p.SenhaHash)
                .HasColumnName("senha_hash")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(p => p.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("DATETIME(6)");

            builder.HasOne(p => p.Estabelecimento)
                .WithOne(e => e.Profissional)
                .HasForeignKey<Estabelecimento>(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}