using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Infrastructure.Configurations
{
    public class AvaliacaoConfiguration : IEntityTypeConfiguration<Avaliacao>
    {
        public void Configure(EntityTypeBuilder<Avaliacao> builder)
        {
            builder.ToTable("avaliacao");

            builder.HasKey(a => a.AvaliacaoId);
            builder.Property(a => a.AvaliacaoId).HasColumnName("avaliacao_id");


            builder.Property(a => a.PedidoId)
                .HasColumnName("pedido_id")
                .IsRequired();

            builder.HasIndex(a => a.AvaliacaoId).IsUnique();

            builder.Property(a => a.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            builder.Property(a => a.Nota)
                .HasColumnName("nota")
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(a => a.Comentario)
                .HasColumnName("comentario")
                .HasColumnType("TEXT");

            builder.Property(a => a.Data)
                .HasColumnName("data")
                .HasColumnType("DATETIME(6)");

            builder.Property(a => a.ImagemAvaliacaoUrl)
                .HasColumnName("Imagem_avaliacao_url")
                .HasMaxLength(255);

            builder.Property(a => a.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(a => a.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("DATETIME(6)");

            builder.Property(a => a.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne(a => a.Pedido)
                .WithOne(p => p.Avaliacao)
                .HasForeignKey<Avaliacao>(a => a.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Cliente)
                .WithMany(c => c.Avaliacoes)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}