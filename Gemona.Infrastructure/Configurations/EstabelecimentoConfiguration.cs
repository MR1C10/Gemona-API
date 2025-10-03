using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Infrastructure.Configurations
{
    public class EstabelecimentoConfiguration : IEntityTypeConfiguration<Estabelecimento>
    {
        public void Configure(EntityTypeBuilder<Estabelecimento> builder)
        {
            builder.ToTable("estabelecimento");

            builder.HasKey(e => e.EstabelecimentoId);
            builder.Property(e => e.EstabelecimentoId).HasColumnName("estabelecimento_id");

            builder.Property(e => e.Nome)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(e => e.Telefone)
                .HasColumnName("telefone")
                .HasMaxLength(20);

            builder.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(e => e.Descricao)
                .HasColumnName("descricao")
                .HasColumnType("TEXT");

            builder.Property(e => e.Cnpj)
                .HasColumnName("cnpj")
                .HasMaxLength(14)
                .HasConversion(
                    cnpj => cnpj.Valor,
                    valor => new Cnpj(valor))
                .IsRequired();

            builder.HasIndex(e => e.Cnpj).IsUnique();

            builder.Property(e => e.ImagemEstabelecimentoUrl)
                .HasColumnName("imagem_estabelecimento_url")
                .HasMaxLength(255);

            builder.Property(e => e.ProfissionalId)
                .HasColumnName("profissional_id")
                .IsRequired();

            builder.Property(e => e.EnderecoId)
                .HasColumnName("endereco_id")
                .IsRequired();

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

            builder.HasOne(e => e.Profissional)
                .WithOne(p => p.Estabelecimento)
                .HasForeignKey<Estabelecimento>(e => e.ProfissionalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Endereco)
                .WithOne(end => end.Estabelecimento)
                .HasForeignKey<Estabelecimento>(e => e.EnderecoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.HorariosFuncionamento)
                .WithOne(h => h.Estabelecimento)
                .HasForeignKey(h => h.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Servicos)
                .WithOne(s => s.Estabelecimento)
                .HasForeignKey(s => s.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}