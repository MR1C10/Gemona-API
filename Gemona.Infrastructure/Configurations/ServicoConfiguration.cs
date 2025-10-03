using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;

namespace Gemona.Infrastructure.Configurations
{
    public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
    {
        public void Configure(EntityTypeBuilder<Servico> builder)
        {
            builder.ToTable("servicos");

            builder.HasKey(s => s.ServicoId);
            builder.Property(s => s.ServicoId).HasColumnName("servico_id");

            builder.Property(s => s.Nome)
                .HasColumnName("nome")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(s => s.Descricao)
                .HasColumnName("descricao")
                .HasColumnType("TEXT");

            builder.Property(s => s.SubCategoriaId)
                .HasColumnName("sub_categoria_id")
                .IsRequired();

            builder.Property(s => s.Preco)
                .HasColumnName("preco")
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(s => s.ImagemServicoUrl)
                .HasColumnName("imagem_servico_url")
                .HasMaxLength(255);

            builder.Property(s => s.EstabelecimentoId)
                .HasColumnName("estabelecimento_id")
                .IsRequired();

            builder.Property(s => s.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(s => s.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("DATETIME(6)");

            builder.Property(s => s.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne(s => s.SubCategoria)
                .WithMany(sc => sc.Servicos)
                .HasForeignKey(s => s.SubCategoriaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Estabelecimento)
                .WithMany(e => e.Servicos)
                .HasForeignKey(s => s.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Pedidos)
                .WithOne(p => p.Servico)
                .HasForeignKey(p => p.ServicoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}