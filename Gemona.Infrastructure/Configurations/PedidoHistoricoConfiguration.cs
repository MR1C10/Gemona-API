using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Infrastructure.Configurations
{
    public class PedidoHistoricoConfiguration : IEntityTypeConfiguration<PedidoHistorico>
    {
        public void Configure(EntityTypeBuilder<PedidoHistorico> builder)
        {
            builder.ToTable("pedido_historico");

            builder.HasKey(p => p.PedidoHistoricoId);
            builder.Property(p => p.PedidoHistoricoId).HasColumnName("pedido_historico_id");

            builder.Property(p => p.PedidoId)
                .HasColumnName("pedido_id")
                .IsRequired();

            builder.Property(p => p.StatusAnterior)
                .HasColumnName("status_anterior")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.StatusNovo)
                .HasColumnName("status_novo")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.DataAlteracao)
                .HasColumnName("data_alteracao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(p => p.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(p => p.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("DATETIME(6)");

            builder.Property(p => p.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne(p => p.Pedido)
                .WithMany(ped => ped.Historicos)
                .HasForeignKey(p => p.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}